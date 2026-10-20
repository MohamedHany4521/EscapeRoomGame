<<<<<<< Updated upstream
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    IInteractable currentInteractable;

    void Update()
    {
        CheckInteraction();
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Debug.Log($"E pressed, Calling Interact() on {currentInteractable}");
            currentInteractable.Interact();
        }
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");

            IInteractable newInteractable = hit.collider.GetComponent<IInteractable>();

            if (newInteractable != null)
            {
                Debug.Log($" Found IInteractable: {hit.collider.name}");

                if (currentInteractable != null && currentInteractable != newInteractable)
                {
                    Debug.Log($"Switching from {currentInteractable} to {newInteractable}");
                    currentInteractable.HideOutline();
                }

                if (newInteractable.IsInteractable)
                {
                    SetNewCurrentInteractable(newInteractable);
                }
            }
            else
            {
                Debug.Log($"No IInteractable found on {hit.collider.name}");
                DisableInteractable(currentInteractable);
            }
        }
        else
        {
            Debug.Log("Raycast didn't hit anything");
            DisableInteractable(currentInteractable);
        }
    }

    void SetNewCurrentInteractable(IInteractable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.ShowOutline();
        Debug.Log($"Showing outline for {newInteractable}");
    }

    void DisableInteractable(IInteractable interactable)
    {
        if (interactable != null)
        {
            interactable.HideOutline();
            Debug.Log($"Hiding outline");
        }
        currentInteractable = null;
    }
=======
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float playerReach = 3f;

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

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
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
                case InteractableType.Generic:
                    cursor = examineCursor;
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
>>>>>>> Stashed changes
}