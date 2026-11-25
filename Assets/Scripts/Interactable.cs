using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline outline;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private UI promptUi;
    [SerializeField] private InteractableType interactionType = InteractableType.Generic;

    private bool isInvoking;

    public string InteractionPrompt => interactionPrompt;
    public virtual bool IsInteractable => gameObject.activeSelf && enabled;
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
        if (outline != null)
        {
            outline.enabled = true;
        }

        if (promptUi != null)
        {
            promptUi.SetInteractionText(interactionPrompt);
        }
    }

    public void HideOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }

        if (promptUi != null)
        {
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
            Debug.Log($"Interacted with: {gameObject.name}");
            onInteract?.Invoke();
        }
        finally
        {
            isInvoking = false;
        }
    }
}
