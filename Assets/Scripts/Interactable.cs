using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline outline;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private InteractableType interactionType = InteractableType.Generic;

    private bool isInvoking;

    public InteractableType InteractionType => interactionType;
    public bool IsInteractable => isActiveAndEnabled;

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
}
