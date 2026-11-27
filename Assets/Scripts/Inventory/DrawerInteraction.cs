using System.Collections;
using UnityEngine;

/// <summary>
/// Reusable, configurable drawer controller for the escape room.
/// Implements IInteractable for interaction via PlayerInteraction raycast.
/// Supports optional key requirement, key consumption, reversible open/close,
/// on-screen locked feedback, independent drawer instances, and scale-safe collider handling.
/// </summary>
public class DrawerInteraction : MonoBehaviour, IInteractable
{
    [Header("Drawer References")]
    [Tooltip("The movable drawer Transform (e.g. SM_FilingCabinetDrawer_01).")]
    [SerializeField] private Transform drawerTransform;

    [Tooltip("The BoxCollider on the drawer used for interaction raycasts.")]
    [SerializeField] private Collider drawerCollider;

    [Tooltip("Optional Outline component for visual hover feedback.")]
    [SerializeField] private Outline outline;

    [Header("State")]
    [Tooltip("Current open state of the drawer.")]
    [SerializeField] private bool isOpen = false;

    [Tooltip("Whether this drawer requires a key to open. If false, opens without a key.")]
    [SerializeField] private bool requiresKey = false;

    [Tooltip("The unique ItemID required in PlayerInventory to unlock this drawer.")]
    [SerializeField] private string requiredKeyID = "";

    [Tooltip("If true, removes the key from inventory upon unlocking. If false, player keeps key.")]
    [SerializeField] private bool consumeKeyOnUnlock = false;

    [Tooltip("Current locked state of the drawer.")]
    [SerializeField] private bool isLocked = false;

    [Tooltip("Whether the player can interact with an open drawer to close it.")]
    [SerializeField] private bool canBeClosed = true;

    [Header("Slide Settings")]
    [Tooltip("Local direction to slide the drawer outward. For FilingCabinet01, local (0, -1, 0) slides toward the handle.")]
    [SerializeField] private Vector3 slideDirection = new Vector3(0f, -1f, 0f);

    [Tooltip("Distance in meters to slide the drawer outward.")]
    [SerializeField] private float slideDistance = 0.2f;

    [Tooltip("Duration in seconds for the opening/closing slide animation.")]
    [SerializeField] private float slideDuration = 1.0f;

    [Header("Interaction Type")]
    [SerializeField] private InteractableType interactionType = InteractableType.Hold;

    private bool isAnimating = false;
    private Vector3 closedLocalPosition;

    public Transform DrawerTransform => drawerTransform;
    public Collider DrawerCollider => drawerCollider;
    public bool RequiresKey => requiresKey;
    public string RequiredKeyID => requiredKeyID;
    public bool ConsumeKeyOnUnlock => consumeKeyOnUnlock;
    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;
    public bool CanBeClosed => canBeClosed;
    public Vector3 SlideDirection => slideDirection;
    public float SlideDistance => slideDistance;
    public float SlideDuration => slideDuration;
    public bool IsInteractable => !isAnimating && (canBeClosed || !isOpen) && enabled && gameObject.activeInHierarchy;
    public InteractableType InteractionType => interactionType;

    private void Awake()
    {
        // 1. Locate drawer Transform if not explicitly assigned
        if (drawerTransform == null)
        {
            drawerTransform = transform;
        }

        // 2. Cache closed position exactly once to prevent positional drift
        closedLocalPosition = drawerTransform.localPosition;

        // 3. Locate drawer collider if not explicitly assigned
        if (drawerCollider == null)
        {
            drawerCollider = GetComponent<Collider>();
            if (drawerCollider == null && drawerTransform != null)
            {
                drawerCollider = drawerTransform.GetComponent<Collider>();
            }
        }

        // 4. Locate outline if not explicitly assigned
        if (outline == null)
        {
            outline = GetComponent<Outline>();
            if (outline == null && drawerTransform != null)
            {
                outline = drawerTransform.GetComponent<Outline>();
            }
        }
    }

    public void Interact()
    {
        if (isAnimating) return;

        if (!isOpen)
        {
            if (isLocked)
            {
                if (!requiresKey)
                {
                    // Drawer does not require a key; unlock and slide open
                    isLocked = false;
                    Debug.Log($"DrawerInteraction on '{gameObject.name}': Drawer does not require a key. Unlocking and opening.", this);
                    StartCoroutine(AnimateDrawer(true));
                    return;
                }

                if (string.IsNullOrWhiteSpace(requiredKeyID))
                {
                    Debug.LogWarning($"DrawerInteraction on '{gameObject.name}': Drawer requires a key, but 'requiredKeyID' is empty. Drawer remains safely locked.", this);
                    ShowFeedback("Drawer is locked.");
                    return;
                }

                PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
                if (inventory == null)
                {
                    Debug.LogWarning($"DrawerInteraction on '{gameObject.name}': No PlayerInventory found in scene. Cannot verify key '{requiredKeyID}'.", this);
                    ShowFeedback($"Requires {requiredKeyID}");
                    return;
                }

                if (inventory.HasItem(requiredKeyID))
                {
                    InventoryItem item = inventory.GetItem(requiredKeyID);
                    string keyName = item != null && !string.IsNullOrWhiteSpace(item.DisplayName) ? item.DisplayName : requiredKeyID;

                    if (consumeKeyOnUnlock)
                    {
                        inventory.RemoveItem(requiredKeyID);
                        Debug.Log($"DrawerInteraction on '{gameObject.name}': Key '{requiredKeyID}' consumed on unlock.", this);
                    }

                    isLocked = false;
                    Debug.Log($"DrawerInteraction on '{gameObject.name}': Key '{requiredKeyID}' verified in inventory. Unlocking and opening drawer.", this);
                    StartCoroutine(AnimateDrawer(true));
                }
                else
                {
                    Debug.LogWarning($"DrawerInteraction on '{gameObject.name}': Drawer is locked. Requires key '{requiredKeyID}'.", this);
                    ShowFeedback($"Requires {requiredKeyID}");
                }
            }
            else
            {
                // Drawer is closed but already unlocked -> open smoothly
                StartCoroutine(AnimateDrawer(true));
            }
        }
        else
        {
            // Drawer is open -> close if allowed
            if (canBeClosed)
            {
                StartCoroutine(AnimateDrawer(false));
            }
        }
    }

    private IEnumerator AnimateDrawer(bool opening)
    {
        isAnimating = true;
        HideOutline();

        Vector3 startPos = drawerTransform.localPosition;

        // Calculate displacement in parent local space
        Vector3 localDir = slideDirection.sqrMagnitude > 0.0001f ? slideDirection.normalized : -Vector3.up;
        Vector3 targetOffset;
        if (drawerTransform.parent != null)
        {
            // Transform local direction into world space, scale by distance in meters, then convert back to parent local space
            Vector3 worldSlideVector = drawerTransform.TransformDirection(localDir) * slideDistance;
            targetOffset = drawerTransform.parent.InverseTransformVector(worldSlideVector);
        }
        else
        {
            targetOffset = localDir * slideDistance;
        }

        Vector3 targetPos = opening ? closedLocalPosition + targetOffset : closedLocalPosition;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            drawerTransform.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }

        drawerTransform.localPosition = targetPos;
        isAnimating = false;
        isOpen = opening;

        Debug.Log($"DrawerInteraction on '{gameObject.name}': Drawer successfully {(opening ? "opened" : "closed")}.", this);
    }

    public void UnlockAndOpen()
    {
        if (isOpen || isAnimating) return;
        isLocked = false;
        StartCoroutine(AnimateDrawer(true));
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void ShowOutline()
    {
        if (outline != null)
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

    private void ShowFeedback(string message)
    {
        UI ui = UI.Instance != null ? UI.Instance : FindFirstObjectByType<UI>();
        if (ui != null)
        {
            ui.ShowInteractionFeedback(message);
        }
    }

    private void OnDisable()
    {
        HideOutline();
    }
}
