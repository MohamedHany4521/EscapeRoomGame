using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    Interactable currentInteractable;
    // Update is called once per frame
    void Update()
    {
        CheckInteraction();
        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    
    }


    void CheckInteraction()
    {
        RaycastHit hit;

        // Use ScreenPointToRay for mouse-based raycasts
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.CompareTag("interactable"))
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (newInteractable != null)
                {
                    if (currentInteractable != null && currentInteractable != newInteractable)
                    {
                        DisableIneractable(currentInteractable);
                    }

                    if (newInteractable.enabled)
                    {
                        SetNewCurrentInteractable(newInteractable);
                    }
                    else
                    {
                        DisableIneractable(currentInteractable);
                    }
                }
            }
        }
        else
        {
            DisableIneractable(currentInteractable);
        }
    }


    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        UI.instance.SetInteractionText("PickUp Money");

        currentInteractable = newInteractable;
        currentInteractable.ShowOutline();



    }

    void DisableIneractable(Interactable newInteractable)
    {

        UI.instance.ClearInteractionText();
        if (newInteractable != null)
        {
            newInteractable.HideOutline();
        }
        currentInteractable = null;
    }
}
