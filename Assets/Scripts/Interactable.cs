using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private Outline outline;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private string interactionPrompt = "Interact";

    

    private bool isInvoking;


    public string InteractionPrompt => interactionPrompt;
    public virtual bool IsInteractable => isActiveAndEnabled;

    InteractableType IInteractable.InteractionType => InteractionType;

    public InteractableType InteractionType;

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
        if (outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
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