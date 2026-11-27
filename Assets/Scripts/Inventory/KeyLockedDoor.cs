using System.Collections;
using UnityEngine;

/// <summary>
/// Reusable, configurable door controller for the escape room.
/// Implements IInteractable for interaction via PlayerInteraction raycast.
/// Supports optional key requirement, key consumption, reversible open/close,
/// on-screen locked feedback, independent door instances, and scale-safe collider handling.
/// </summary>
public class KeyLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Key Configuration")]
    [Tooltip("Whether this door requires a key to open. If false, opens without a key.")]
    [SerializeField] private bool requiresKey = true;

    [Tooltip("The unique ItemID required in PlayerInventory to unlock this door.")]
    [SerializeField] private string requiredKeyID = "Key_Door1";

    [Tooltip("If true, removes the key from inventory upon unlocking. If false, player keeps key.")]
    [SerializeField] private bool consumeKeyOnUnlock = false;

    [Header("Door References")]
    [Tooltip("The movable door panel child Transform (e.g. SM_Door_01). Frame remains stationary.")]
    [SerializeField] private Transform doorPanel;

    [Tooltip("The BoxCollider on the door panel that blocks passage when closed and clears when open.")]
    [SerializeField] private BoxCollider doorPanelCollider;

    [Tooltip("The static MeshCollider on the door frame.")]
    [SerializeField] private MeshCollider doorFrameCollider;

    [Tooltip("Outline component for interaction visual feedback.")]
    [SerializeField] private Outline outline;

    [Header("Animation Settings")]
    [Tooltip("Local axis of rotation around the panel hinge pivot. (0,0,1) corresponds to World Up for FBX doors.")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Angle in degrees to swing the door open.")]
    [SerializeField] private float openAngle = 90f;

    [Tooltip("Duration in seconds for the opening/closing animation.")]
    [SerializeField] private float openDuration = 1.2f;

    [Tooltip("Whether the player can interact with an open door to close it.")]
    [SerializeField] private bool canBeClosed = true;

    [Header("State")]
    [Tooltip("Current locked state of the door.")]
    [SerializeField] private bool isLocked = true;

    [Tooltip("Current open state of the door.")]
    [SerializeField] private bool isOpen = false;

    [Header("Interaction Type")]
    [SerializeField] private InteractableType interactionType = InteractableType.Hold;

    private bool isAnimating = false;
    private Quaternion closedLocalRotation;

    public bool RequiresKey => requiresKey;
    public string RequiredKeyID => requiredKeyID;
    public bool ConsumeKeyOnUnlock => consumeKeyOnUnlock;
    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;
    public bool CanBeClosed => canBeClosed;
    public bool IsInteractable => !isAnimating && (canBeClosed || !isOpen) && enabled && gameObject.activeInHierarchy;
    public InteractableType InteractionType => interactionType;

    private void Awake()
    {
        // 1. Locate door panel child if not explicitly assigned
        if (doorPanel == null)
        {
            if (doorPanelCollider != null)
            {
                doorPanel = doorPanelCollider.transform;
            }
            else
            {
                // Search for any movable door leaf child (e.g. SM_Door_01, SM_Door_02, DoorPanel)
                foreach (Transform child in GetComponentsInChildren<Transform>(true))
                {
                    if (child != transform && (child.name.Contains("Door_") || child.name.Contains("DoorPanel") || child.name.Contains("DoorLeaf")))
                    {
                        doorPanel = child;
                        break;
                    }
                }

                if (doorPanel == null)
                {
                    foreach (Transform child in GetComponentsInChildren<Transform>(true))
                    {
                        if (child != transform && child.name.Contains("Door") && !child.name.Contains("Frame"))
                        {
                            doorPanel = child;
                            break;
                        }
                    }
                }

                if (doorPanel == null)
                {
                    doorPanel = transform;
                }
            }
        }

        // 2. Cache closed rotation exactly once
        if (doorPanel != null)
        {
            closedLocalRotation = doorPanel.localRotation;
        }

        // 3. Configure/ensure door panel BoxCollider
        if (doorPanelCollider == null && doorPanel != null)
        {
            doorPanelCollider = doorPanel.GetComponent<BoxCollider>();
            if (doorPanelCollider == null)
            {
                doorPanelCollider = doorPanel.gameObject.AddComponent<BoxCollider>();

                // Account for potential 100x model import scale without breaking bounds
                float scaleX = Mathf.Abs(doorPanel.lossyScale.x);
                if (scaleX > 10f)
                {
                    doorPanelCollider.center = new Vector3(-0.00324f, 0f, 0.00003f);
                    doorPanelCollider.size = new Vector3(0.0066f, 0.002f, 0.0147f);
                }
                else
                {
                    doorPanelCollider.center = new Vector3(-0.324f, 0f, 0.003f);
                    doorPanelCollider.size = new Vector3(0.66f, 0.20f, 1.47f);
                }
            }
        }

        if (doorPanelCollider != null)
        {
            doorPanelCollider.isTrigger = false;
            doorPanelCollider.enabled = !isOpen;
        }

        // 4. Configure/ensure frame MeshCollider (do not automatically add one)
        if (doorFrameCollider == null)
        {
            doorFrameCollider = GetComponent<MeshCollider>();
        }

        // 5. Configure/ensure Outline
        if (outline == null)
        {
            if (doorPanel != null)
                outline = doorPanel.GetComponent<Outline>();

            if (outline == null)
                outline = GetComponent<Outline>();

            if (outline == null && doorPanel != null)
                outline = doorPanel.gameObject.AddComponent<Outline>();
        }

        if (outline != null)
        {
            outline.enabled = false;
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
        if (outline == null && doorPanel != null)
        {
            outline = doorPanel.GetComponent<Outline>();
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
        if (!IsInteractable || isAnimating)
        {
            return;
        }

        if (!isOpen)
        {
            // Door is currently closed
            if (isLocked)
            {
                if (!requiresKey)
                {
                    // Door does not require a key; unlocks and opens directly!
                    isLocked = false;
                    Debug.Log($"KeyLockedDoor on '{gameObject.name}': Door does not require a key. Unlocking and opening.", this);
                    StartCoroutine(AnimateDoor(true));
                    return;
                }

                if (string.IsNullOrWhiteSpace(requiredKeyID))
                {
                    Debug.LogWarning($"KeyLockedDoor on '{gameObject.name}': Door requires a key, but 'requiredKeyID' is empty. Door remains locked.", this);
                    ShowFeedback("Door is locked.");
                    return;
                }

                PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
                if (inventory == null)
                {
                    Debug.LogWarning($"KeyLockedDoor on '{gameObject.name}': No PlayerInventory found in scene. Cannot verify key '{requiredKeyID}'.", this);
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
                        Debug.Log($"KeyLockedDoor on '{gameObject.name}': Key '{requiredKeyID}' consumed on unlock.", this);
                    }

                    isLocked = false;
                    Debug.Log($"KeyLockedDoor on '{gameObject.name}': Key '{requiredKeyID}' verified in inventory. Unlocking and opening door.", this);
                    StartCoroutine(AnimateDoor(true));
                }
                else
                {
                    Debug.LogWarning($"KeyLockedDoor on '{gameObject.name}': Door is locked. Requires key '{requiredKeyID}'.", this);
                    ShowFeedback($"Requires {requiredKeyID}");
                }
            }
            else
            {
                // Door is closed but already unlocked -> open smoothly
                StartCoroutine(AnimateDoor(true));
            }
        }
        else
        {
            // Door is open -> close if allowed
            if (canBeClosed)
            {
                StartCoroutine(AnimateDoor(false));
            }
        }
    }

    private IEnumerator AnimateDoor(bool opening)
    {
        isAnimating = true;
        HideOutline();

        Quaternion startRot = doorPanel.localRotation;
        Quaternion targetRot = opening
            ? closedLocalRotation * Quaternion.AngleAxis(openAngle, rotationAxis)
            : closedLocalRotation;

        // If closing, ensure collider stays disabled during transit so it does not snag or push player
        if (!opening && doorPanelCollider != null)
        {
            doorPanelCollider.enabled = false;
        }

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            doorPanel.localRotation = Quaternion.Slerp(startRot, targetRot, smoothT);
            yield return null;
        }

        doorPanel.localRotation = targetRot;
        isAnimating = false;
        isOpen = opening;

        // Collider handling after animation:
        // When open -> disable panel collider so the player can pass freely
        // When closed -> enable solid BoxCollider to block passage
        if (doorPanelCollider == null && doorPanel != null)
        {
            doorPanelCollider = doorPanel.GetComponent<BoxCollider>();
        }

        if (doorPanelCollider != null)
        {
            doorPanelCollider.enabled = !isOpen;
            doorPanelCollider.isTrigger = false;
        }

        Debug.Log($"KeyLockedDoor on '{gameObject.name}': Door successfully {(opening ? "opened" : "closed")}.", this);
    }

    public void UnlockAndOpen()
    {
        if (isOpen || isAnimating) return;
        isLocked = false;
        StartCoroutine(AnimateDoor(true));
    }

    public void Unlock()
    {
        isLocked = false;
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
