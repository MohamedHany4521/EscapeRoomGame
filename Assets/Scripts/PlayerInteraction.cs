using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float playerReach = 3.5f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool allowMouseClick = true;

    [Header("Dynamic Cursor UI")]
    [SerializeField] private GameObject normalCursor;
    [SerializeField] private GameObject adjustCursor;
    [SerializeField] private GameObject holdCursor;
    [SerializeField] private GameObject examineCursor;

    private IInteractable currentInteractable;
    private GameObject activeCursor;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
            playerCamera = GetComponent<Camera>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: No player camera assigned or found.", this);

        ShowCursor(null);
    }

    private void Update()
    {
        CheckInteraction();

        bool interactTriggered = Input.GetKeyDown(interactionKey) || (allowMouseClick && Input.GetMouseButtonDown(0));

        if (interactTriggered &&
            currentInteractable != null &&
            currentInteractable.IsInteractable)
        {
            currentInteractable.Interact();
        }
    }

    private void CheckInteraction()
    {
        if (playerCamera == null || !playerCamera.enabled)
        {
            SetCurrentInteractable(null);
            ShowCursor(null);
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        IInteractable detectedInteractable = null;

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            MonoBehaviour[] behaviours =
                hit.collider.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable &&
                    interactable.IsInteractable)
                {
                    detectedInteractable = interactable;
                    break;
                }
            }
        }

        if (detectedInteractable != null)
        {
            SetCurrentInteractable(detectedInteractable);
            ShowCursor(detectedInteractable.InteractionType);
        }
        else
        {
            SetCurrentInteractable(null);
            ShowCursor(null);
        }
    }

    private void SetCurrentInteractable(IInteractable target)
    {
        if (ReferenceEquals(currentInteractable, target))
            return;

        if (currentInteractable != null)
            currentInteractable.HideOutline();

        currentInteractable = target;

        if (currentInteractable != null)
            currentInteractable.ShowOutline();
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

    private void OnDisable()
    {
        SetCurrentInteractable(null);
        ShowCursor(null);
    }
}
