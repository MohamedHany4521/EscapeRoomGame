using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private Outline outline;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private UI promptUi; 
    [SerializeField] private InteractableType interactionType;

    private bool isInvoking;

    public string InteractionPrompt => interactionPrompt;
    public virtual bool IsInteractable => isActiveAndEnabled;
    public InteractableType InteractionType => interactionType;

    private void OnEnable()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();

            if (outline == null)
                outline = GetComponentInChildren<Outline>(true);
        }

        HideOutline();
    }

    private void OnDisable()
    {
        HideOutline();
    }

    public void ShowOutline()
    {
        if (outline != null && promptUi != null)
        {
            outline.enabled = true;
            promptUi.SetInteractionText(interactionPrompt);
        }

    }

    public void HideOutline()
    {
        if (outline != null && promptUi != null)
        {
            outline.enabled = false;
            promptUi.ClearInteractionText();
        }
            
    }


    public virtual void Interact()

    {
        if (!IsInteractable || isInvoking)
            return;

        isInvoking = true;

        try
        {
            onInteract?.Invoke();
        }
        finally
        {
            isInvoking = false;
        }
    }
}
