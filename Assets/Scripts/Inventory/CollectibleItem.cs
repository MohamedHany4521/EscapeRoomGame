using UnityEngine;

/// <summary>
/// Represents a world item that can be highlighted and collected into the player's inventory via interaction.
/// Implements IInteractable to integrate seamlessly with the existing PlayerInteraction raycast system.
/// </summary>
public class CollectibleItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    [Tooltip("Unique identifier for this item in the inventory system.")]
    [SerializeField] private string itemID;

    [Tooltip("Human-readable name displayed in UI or logs.")]
    [SerializeField] private string displayName;

    [Tooltip("Tag/category identifying the item type (e.g., Key, Battery, Document).")]
    [SerializeField] private string itemTag;

    [Tooltip("Optional detailed description of the item.")]
    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Visual Feedback")]
    [Tooltip("Outline component used for highlighting when looked at.")]
    [SerializeField] private Outline outline;

    private bool isCollected = false;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public string ItemTag => itemTag;
    public string Description => description;
    public Outline ItemOutline => outline;
    public bool IsCollected => isCollected;

    public bool IsInteractable => !isCollected && enabled && gameObject.activeInHierarchy;

    private void Awake()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }

        if (outline != null)
        {
            outline.enabled = false;
        }

        // Fallback: If itemTag is empty, try using the GameObject's Unity tag if valid
        if (string.IsNullOrWhiteSpace(itemTag) && !string.IsNullOrWhiteSpace(gameObject.tag) && gameObject.tag != "Untagged")
        {
            itemTag = gameObject.tag;
        }
    }

    private void Start()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void ShowOutline()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }

        if (outline != null && IsInteractable)
        {
            outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Interact()
    {
        if (!IsInteractable || isCollected)
        {
            return;
        }

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning($"CollectibleItem on '{gameObject.name}': No PlayerInventory found in scene. Cannot collect item '{displayName}' ({itemID}).", this);
            return;
        }

        InventoryItem item = new InventoryItem(itemID, displayName, itemTag, description);
        bool added = inventory.AddItem(item);

        if (added)
        {
            isCollected = true;
            HideOutline();

            // Disable collider immediately to prevent further raycast hits before object destruction
            if (TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }

            Debug.Log($"CollectibleItem: Successfully collected '{displayName}' ({itemID}). Destroying world object.", this);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"CollectibleItem on '{gameObject.name}': Failed to add '{displayName}' ({itemID}) to inventory. Object preserved in world.", this);
        }
    }

    private void OnDisable()
    {
        HideOutline();
    }
}
