using System.Collections;
using UnityEngine;

/// <summary>
/// Controls a key-locked door in the escape room.
/// Implements IInteractable for interaction via PlayerInteraction raycast.
/// Requires Key_Door1 in PlayerInventory to unlock and swing open.
/// Only animates the movable door panel child (SM_Door_01), keeping the frame stationary.
/// </summary>
public class KeyLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Key Configuration")]
    [Tooltip("The unique ItemID required in PlayerInventory to unlock this door.")]
    [SerializeField] private string requiredKeyID = "Key_Door1";

    [Header("Door References")]
    [Tooltip("The movable door panel child Transform (SM_Door_01). Frame remains stationary.")]
    [SerializeField] private Transform doorPanel;

    [Tooltip("The BoxCollider on the door panel that blocks passage when closed and moves when open.")]
    [SerializeField] private BoxCollider doorPanelCollider;

    [Tooltip("The static MeshCollider on the door frame.")]
    [SerializeField] private MeshCollider doorFrameCollider;

    [Tooltip("Outline component for interaction visual feedback.")]
    [SerializeField] private Outline outline;

    [Header("Animation Settings")]
    [Tooltip("Local axis of rotation around the panel hinge pivot. (0,0,1) corresponds to World Up.")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Angle in degrees to swing the door open.")]
    [SerializeField] private float openAngle = 90f;

    [Tooltip("Duration in seconds for the opening animation.")]
    [SerializeField] private float openDuration = 1.2f;

    [Header("State")]
    [SerializeField] private bool isLocked = true;
    [SerializeField] private bool isOpen = false;

    private bool isOpening = false;
    private Quaternion closedLocalRotation;

    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;
    public bool IsInteractable => !isOpen && !isOpening && enabled && gameObject.activeInHierarchy;

    private void Awake()
    {
        // 1. Locate door panel child if not explicitly assigned
        if (doorPanel == null)
        {
            Transform found = transform.Find("SM_Door_01");
            if (found == null)
            {
                foreach (Transform child in GetComponentsInChildren<Transform>(true))
                {
                    if (child != transform && child.name.Equals("SM_Door_01", System.StringComparison.OrdinalIgnoreCase))
                    {
                        found = child;
                        break;
                    }
                }
            }

            doorPanel = found != null ? found : transform;
        }

        // 2. Cache closed rotation
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
                doorPanelCollider.center = new Vector3(-0.324f, 0f, 0.003f);
                doorPanelCollider.size = new Vector3(0.66f, 0.20f, 1.47f);
            }
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
        if (!IsInteractable || isOpening || isOpen)
        {
            return;
        }

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning($"KeyLockedDoor on '{gameObject.name}': No PlayerInventory found in scene. Cannot verify key '{requiredKeyID}'.", this);
            return;
        }

        if (inventory.HasItem(requiredKeyID))
        {
            Debug.Log($"KeyLockedDoor on '{gameObject.name}': Key '{requiredKeyID}' verified in inventory. Unlocking and opening door.", this);
            isLocked = false;
            StartCoroutine(OpenDoorRoutine());
        }
        else
        {
            Debug.LogWarning($"KeyLockedDoor on '{gameObject.name}': Door is locked. Requires key '{requiredKeyID}'.", this);
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        isOpening = true;
        HideOutline();

        Quaternion startRot = doorPanel.localRotation;
        Quaternion targetRot = closedLocalRotation * Quaternion.AngleAxis(openAngle, rotationAxis);

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
        isOpening = false;
        isOpen = true;

        // Disable panel collider after opening so the player can pass freely
        if (doorPanelCollider == null && doorPanel != null)
        {
            doorPanelCollider = doorPanel.GetComponent<BoxCollider>();
        }

        if (doorPanelCollider != null)
        {
            doorPanelCollider.enabled = false;
        }

        Debug.Log($"KeyLockedDoor on '{gameObject.name}': Door successfully opened.", this);
    }

    private void OnDisable()
    {
        HideOutline();
    }
}
