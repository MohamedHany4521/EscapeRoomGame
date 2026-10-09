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
}
