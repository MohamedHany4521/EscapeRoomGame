using UnityEngine;

/// <summary>
/// World inspection point for reading documents.
/// Implements IInteractable for interaction via the PlayerInteraction raycast system.
/// Requires the document item (default: Document_01) in PlayerInventory to open the reading panel.
/// </summary>
public class DocumentReaderInteraction : MonoBehaviour, IInteractable
{
    [Header("Document Configuration")]
    [Tooltip("The unique ItemID required in PlayerInventory to read at this point.")]
    [SerializeField] private string requiredDocumentID = "Document_01";

    [Header("UI Reference")]
    [Tooltip("Reference to the DocumentInspectionUI controller.")]
    [SerializeField] private DocumentInspectionUI inspectionUI;

    [Header("Visual Feedback")]
    [Tooltip("Outline component used for highlighting when looked at.")]
    [SerializeField] private Outline outline;

    [Header("Interaction Type")]
    [SerializeField] private InteractableType interactionType = InteractableType.Examine;

    public string RequiredDocumentID => requiredDocumentID;
    public DocumentInspectionUI InspectionUI => inspectionUI;

    public bool IsInteractable => enabled && gameObject.activeInHierarchy && (inspectionUI == null || !inspectionUI.IsOpen);
    public InteractableType InteractionType => interactionType;

    private void Awake()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
            if (outline == null)
            {
                outline = GetComponentInChildren<Outline>();
            }
        }

        if (outline != null)
        {
            outline.enabled = false;
        }

        if (inspectionUI == null)
        {
            inspectionUI = DocumentInspectionUI.Instance != null ? DocumentInspectionUI.Instance : FindFirstObjectByType<DocumentInspectionUI>();
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

    /// <summary>
    /// Handles player interaction via E-key or left-click raycast.
    /// Verifies that the required document is present in inventory before opening the reading UI.
    /// </summary>
    public void Interact()
    {
        if (!IsInteractable)
        {
            return;
        }

        if (inspectionUI == null)
        {
            inspectionUI = DocumentInspectionUI.Instance != null ? DocumentInspectionUI.Instance : FindFirstObjectByType<DocumentInspectionUI>();
        }

        if (inspectionUI != null && inspectionUI.IsOpen)
        {
            return;
        }

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning($"DocumentReaderInteraction on '{gameObject.name}': No PlayerInventory found in scene. Cannot verify document '{requiredDocumentID}'.", this);
            return;
        }

        if (!inventory.HasItem(requiredDocumentID))
        {
            Debug.LogWarning($"DocumentReaderInteraction on '{gameObject.name}': Document inspection point requires document '{requiredDocumentID}' in inventory to read.", this);
            return;
        }

        InventoryItem docItem = inventory.GetItem(requiredDocumentID);
        if (docItem == null)
        {
            Debug.LogWarning($"DocumentReaderInteraction on '{gameObject.name}': Failed to retrieve item data for '{requiredDocumentID}' from inventory.", this);
            return;
        }

        if (inspectionUI != null)
        {
            HideOutline();
            inspectionUI.OpenDocument(docItem.DisplayName, docItem.Description);
            Debug.Log($"DocumentReaderInteraction on '{gameObject.name}': Opening document '{docItem.DisplayName}' ({requiredDocumentID}).", this);
        }
        else
        {
            Debug.LogError($"DocumentReaderInteraction on '{gameObject.name}': No DocumentInspectionUI found in scene to display document.", this);
        }
    }

    private void OnDisable()
    {
        HideOutline();
    }
}
