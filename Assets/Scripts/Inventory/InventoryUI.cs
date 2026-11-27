using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visible Inventory UI HUD in the escape room.
/// Displays collected items, updates automatically on collection/consumption via PlayerInventory events,
/// shows item name and icon (with fallback icons for keys, documents, batteries),
/// and can be toggled cleanly via Tab or I without breaking player movement or camera control.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Serializable]
    public class SlotUI
    {
        [Tooltip("Root GameObject of this slot.")]
        public GameObject root;

        [Tooltip("Image component displaying the item icon.")]
        public Image iconImage;

        [Tooltip("TextMeshPro component displaying the item display name.")]
        public TextMeshProUGUI nameText;

        [Tooltip("Optional TextMeshPro component displaying the item tag/category.")]
        public TextMeshProUGUI tagText;

        [Tooltip("Optional Image component for slot background.")]
        public Image backgroundImage;
    }

    [Header("Inventory Source")]
    [Tooltip("Reference to the PlayerInventory component. Auto-discovered if null.")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("UI Structure")]
    [Tooltip("Root panel GameObject of the Inventory HUD. Toggled via Tab/I.")]
    [SerializeField] private GameObject panelRoot;

    [Tooltip("CanvasGroup on the panel for smooth visibility toggling without destroying layout.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Parent transform holding the item slot GameObjects.")]
    [SerializeField] private Transform itemsContainer;

    [Tooltip("Optional header text displaying 'INVENTORY [Tab]'.")]
    [SerializeField] private TextMeshProUGUI headerText;

    [Tooltip("Optional text shown when the inventory contains no items (e.g. 'Empty').")]
    [SerializeField] private TextMeshProUGUI emptyText;

    [Header("Slots Configuration")]
    [Tooltip("Pre-created slot UI elements in the hierarchy for immediate zero-garbage display.")]
    [SerializeField] private List<SlotUI> preconfiguredSlots = new List<SlotUI>();

    [Tooltip("Optional prefab/template to instantiate if item count exceeds preconfigured slots.")]
    [SerializeField] private GameObject slotTemplate;

    [Header("Default Fallback Icons")]
    [Tooltip("Default icon used when item is a Key and has no custom icon.")]
    [SerializeField] private Sprite defaultKeyIcon;

    [Tooltip("Default icon used when item is a Document and has no custom icon.")]
    [SerializeField] private Sprite defaultDocumentIcon;

    [Tooltip("Default icon used when item is a Battery and has no custom icon.")]
    [SerializeField] private Sprite defaultBatteryIcon;

    [Tooltip("Default generic item icon.")]
    [SerializeField] private Sprite defaultGenericIcon;

    [Header("Visibility & Input Settings")]
    [Tooltip("Primary key to toggle inventory visibility.")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Tooltip("Secondary key to toggle inventory visibility.")]
    [SerializeField] private KeyCode alternateToggleKey = KeyCode.I;

    [Tooltip("Whether the inventory starts visible on game load.")]
    [SerializeField] private bool startVisible = true;

    [Tooltip("Automatically make inventory visible when a new item is picked up.")]
    [SerializeField] private bool autoShowOnCollect = true;

    private bool isVisible = true;
    private int previousItemCount = 0;
    private readonly List<SlotUI> activeSlots = new List<SlotUI>();
    private readonly List<GameObject> dynamicallyCreatedRoots = new List<GameObject>();

    public bool IsVisible => isVisible;
    public int DisplayedItemCount => previousItemCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        ResolveReferences();
        InitializeSlots();
    }

    private void Start()
    {
        ResolveReferences();
        SubscribeToInventory();

        SetVisible(startVisible);
        RefreshUI();
    }

    private void OnEnable()
    {
        SubscribeToInventory();
    }

    private void OnDisable()
    {
        UnsubscribeFromInventory();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInventory();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // Toggle HUD on Tab or I without disrupting mouse look, movement, or raycast interactions
        if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(alternateToggleKey))
        {
            ToggleVisibility();
        }

        // Lazy reconnection if inventory was instantiated after Awake
        if (playerInventory == null)
        {
            ResolveReferences();
            if (playerInventory != null)
            {
                SubscribeToInventory();
                RefreshUI();
            }
        }
    }

    /// <summary>
    /// Auto-discovers PlayerInventory and panel components if not explicitly assigned in Inspector.
    /// Also ensures proper UI layout components are present.
    /// </summary>
    public void ResolveReferences()
    {
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        }

        if (panelRoot == null)
        {
            Transform found = transform.Find("InventoryPanel");
            panelRoot = found != null ? found.gameObject : gameObject;
        }

        if (canvasGroup == null && panelRoot != null)
        {
            canvasGroup = panelRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panelRoot.AddComponent<CanvasGroup>();
            }
        }

        if (itemsContainer == null && panelRoot != null)
        {
            Transform container = panelRoot.transform.Find("ItemsContainer");
            itemsContainer = container != null ? container : panelRoot.transform;
        }

        if (itemsContainer != null)
        {
            VerticalLayoutGroup vlg = itemsContainer.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = itemsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            vlg.spacing = 8f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
        }
    }

    private void InitializeSlots()
    {
        activeSlots.Clear();

        // 1. Add preconfigured slots from Inspector or search in itemsContainer
        if (preconfiguredSlots != null && preconfiguredSlots.Count > 0)
        {
            activeSlots.AddRange(preconfiguredSlots);
        }
        else if (itemsContainer != null)
        {
            // Auto-discover any child slot GameObjects matching 'Slot'
            for (int i = 0; i < itemsContainer.childCount; i++)
            {
                Transform child = itemsContainer.GetChild(i);
                if (child.name.StartsWith("Slot", StringComparison.OrdinalIgnoreCase) || child.name.StartsWith("ItemSlot", StringComparison.OrdinalIgnoreCase))
                {
                    SlotUI discovered = new SlotUI
                    {
                        root = child.gameObject,
                        iconImage = child.Find("Icon")?.GetComponent<Image>(),
                        nameText = child.Find("Name")?.GetComponent<TextMeshProUGUI>(),
                        tagText = child.Find("Tag")?.GetComponent<TextMeshProUGUI>(),
                        backgroundImage = child.GetComponent<Image>()
                    };

                    if (discovered.nameText == null)
                    {
                        discovered.nameText = child.GetComponentInChildren<TextMeshProUGUI>();
                    }

                    activeSlots.Add(discovered);
                }
            }
        }

        // Configure layout and hide all slots initially until refreshed
        foreach (var slot in activeSlots)
        {
            if (slot.root != null)
            {
                LayoutElement le = slot.root.GetComponent<LayoutElement>();
                if (le == null)
                {
                    le = slot.root.AddComponent<LayoutElement>();
                }
                le.preferredHeight = 52f;
                le.minHeight = 52f;

                slot.root.SetActive(false);
            }
        }
    }

    private void SubscribeToInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
        }
    }

    private void HandleInventoryChanged()
    {
        int currentCount = playerInventory != null ? playerInventory.GetItems().Count : 0;

        // Auto-show when picking up a new item
        if (autoShowOnCollect && currentCount > previousItemCount)
        {
            SetVisible(true);
        }

        RefreshUI();
    }

    /// <summary>
    /// Re-renders all inventory slots according to the current items in PlayerInventory.
    /// Safely hides consumed items and activates collected items.
    /// </summary>
    public void RefreshUI()
    {
        if (playerInventory == null)
        {
            ResolveReferences();
        }

        IReadOnlyList<InventoryItem> items = playerInventory != null ? playerInventory.GetItems() : Array.Empty<InventoryItem>();
        int count = items != null ? items.Count : 0;

        // Update empty state text
        if (emptyText != null)
        {
            emptyText.gameObject.SetActive(count == 0);
        }

        // Ensure we have enough slot instances
        while (activeSlots.Count < count && slotTemplate != null && itemsContainer != null)
        {
            GameObject newSlotObj = Instantiate(slotTemplate, itemsContainer);
            newSlotObj.name = $"Slot_{activeSlots.Count}";
            dynamicallyCreatedRoots.Add(newSlotObj);

            SlotUI newSlot = new SlotUI
            {
                root = newSlotObj,
                iconImage = newSlotObj.transform.Find("Icon")?.GetComponent<Image>(),
                nameText = newSlotObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>(),
                tagText = newSlotObj.transform.Find("Tag")?.GetComponent<TextMeshProUGUI>(),
                backgroundImage = newSlotObj.GetComponent<Image>()
            };

            if (newSlot.nameText == null)
            {
                newSlot.nameText = newSlotObj.GetComponentInChildren<TextMeshProUGUI>();
            }

            activeSlots.Add(newSlot);
        }

        // Bind items to active slots
        for (int i = 0; i < activeSlots.Count; i++)
        {
            SlotUI slot = activeSlots[i];
            if (slot.root == null) continue;

            if (i < count)
            {
                InventoryItem item = items[i];
                slot.root.SetActive(true);

                // Set Name
                if (slot.nameText != null)
                {
                    slot.nameText.text = !string.IsNullOrWhiteSpace(item.DisplayName) ? item.DisplayName : item.ItemID;
                }

                // Set Tag / Category
                if (slot.tagText != null)
                {
                    slot.tagText.text = !string.IsNullOrWhiteSpace(item.ItemTag) ? item.ItemTag : string.Empty;
                }

                // Set Icon
                if (slot.iconImage != null)
                {
                    Sprite resolvedSprite = ResolveItemSprite(item);
                    if (resolvedSprite != null)
                    {
                        slot.iconImage.sprite = resolvedSprite;
                        slot.iconImage.enabled = true;
                    }
                    else
                    {
                        // Fallback: hide icon image or keep generic color
                        slot.iconImage.enabled = false;
                    }
                }
            }
            else
            {
                // Slot is beyond current item count: deactivate so consumed items vanish immediately
                slot.root.SetActive(false);
            }
        }

        previousItemCount = count;
    }

    /// <summary>
    /// Resolves the appropriate sprite for an inventory item, checking custom icon first,
    /// then tag/ID-based default icons, then generic fallback.
    /// </summary>
    private Sprite ResolveItemSprite(InventoryItem item)
    {
        if (item == null) return null;

        // 1. Explicit item icon
        if (item.Icon != null)
        {
            return item.Icon;
        }

        string tag = item.ItemTag ?? string.Empty;
        string id = item.ItemID ?? string.Empty;

        // 2. Key matching
        if (tag.IndexOf("key", StringComparison.OrdinalIgnoreCase) >= 0 ||
            id.IndexOf("key", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (defaultKeyIcon != null) return defaultKeyIcon;
        }

        // 3. Document matching
        if (tag.IndexOf("document", StringComparison.OrdinalIgnoreCase) >= 0 ||
            tag.IndexOf("doc", StringComparison.OrdinalIgnoreCase) >= 0 ||
            id.IndexOf("document", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (defaultDocumentIcon != null) return defaultDocumentIcon;
        }

        // 4. Battery matching
        if (tag.IndexOf("battery", StringComparison.OrdinalIgnoreCase) >= 0 ||
            tag.IndexOf("bat", StringComparison.OrdinalIgnoreCase) >= 0 ||
            id.IndexOf("battery", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (defaultBatteryIcon != null) return defaultBatteryIcon;
        }

        // 5. Generic fallback
        return defaultGenericIcon;
    }

    /// <summary>
    /// Toggles the visibility of the inventory HUD without affecting player controls.
    /// </summary>
    public void ToggleVisibility()
    {
        SetVisible(!isVisible);
    }

    /// <summary>
    /// Shows or hides the inventory HUD cleanly.
    /// </summary>
    public void SetVisible(bool visible)
    {
        isVisible = visible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = false; // Does not block player raycasts/interactions
        }

        if (panelRoot != null && canvasGroup == null)
        {
            panelRoot.SetActive(visible);
        }
    }
}
