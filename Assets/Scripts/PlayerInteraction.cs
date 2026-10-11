
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float playerReach = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private IInteractable currentInteractable;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: No player camera assigned or found.", this);
    }

    private void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(interactionKey) &&
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
            ClearCurrentInteractable();
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

        if (currentInteractable == detectedInteractable)
            return;

        ClearCurrentInteractable();

        if (detectedInteractable != null)
        {
            currentInteractable = detectedInteractable;
            currentInteractable.ShowOutline();
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
            currentInteractable.HideOutline();

        currentInteractable = null;
    }

    private void OnDisable()
    {
        ClearCurrentInteractable();
    }
}
