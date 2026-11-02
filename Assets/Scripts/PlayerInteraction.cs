using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float playerReach = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Cursor UI")]
    [SerializeField] private GameObject normalCursor;
    [SerializeField] private GameObject adjustCursor;
    [SerializeField] private GameObject holdCursor;
    [SerializeField] private GameObject examineCursor;

    private IInteractable currentInteractable;
    private GameObject activeCursor;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        ShowCursor(null);
    }

    private void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(interactKey) && currentInteractable != null && currentInteractable.IsInteractable)
            currentInteractable.Interact();
    }

    private void OnDisable()
    {
        SetCurrentInteractable(null);
        ShowCursor(null);
    }

    private void CheckInteraction()
    {
        if (playerCamera != null)
        {
            Ray ray = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
            {
                IInteractable target =
                    hit.collider.GetComponentInParent<IInteractable>();

                if (target != null && target.IsInteractable)
                {
                    SetCurrentInteractable(target);
                    ShowCursor(target.InteractionType);
                    return;
                }
            }
        }

        SetCurrentInteractable(null);
        ShowCursor(null);
    }

    private void SetCurrentInteractable(IInteractable target)
    {
        if (ReferenceEquals(currentInteractable, target))
        {
            UpdateInteractionPrompt(target);
            return;
        }

        if (currentInteractable != null)
            currentInteractable.HideOutline();

        currentInteractable = target;

        UpdateInteractionPrompt(target);

        if (currentInteractable != null)
            currentInteractable.ShowOutline();
    }

    private void UpdateInteractionPrompt(IInteractable target)
    {
        if (UI.instance == null)
            return;

        if (target == null || string.IsNullOrWhiteSpace(target.InteractionPrompt))
        {
            UI.instance.ClearInteractionText();
            return;
        }

        UI.instance.SetInteractionText($"Press {interactKey} to {target.InteractionPrompt}");
    }

    private void ShowCursor(InteractableType? type)
    {
        GameObject cursor = normalCursor;

        if (type.HasValue)
        {
            switch (type.Value)
            {
                case InteractableType.Adjust:
                    cursor = adjustCursor;
                    break;

                case InteractableType.Hold:
                    cursor = holdCursor;
                    break;

                case InteractableType.Examine:
                    cursor = examineCursor;
                    break;
                case InteractableType.Generic:
                    cursor = normalCursor;
                    break;
            }
        }

        if (activeCursor == cursor)
            return;

        if (activeCursor != null)
            activeCursor.SetActive(false);

        activeCursor = cursor;

        if (activeCursor != null)
            activeCursor.SetActive(true);
    }
}