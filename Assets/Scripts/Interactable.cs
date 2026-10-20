using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline outline;
    [SerializeField] private UnityEvent onInteract;
    private bool isInvoking;
    [SerializeField] private InteractableType interactionType = InteractableType.Generic;

    public InteractableType InteractionType => interactionType;

    private void OnEnable()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
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
        if (isInvoking)
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

    public bool IsInteractable => gameObject.activeSelf && enabled;
}
