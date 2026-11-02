using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private Outline outline;
    [SerializeField] private UnityEvent onInteract;
<<<<<<< Updated upstream
=======
    [SerializeField] private string interactionPrompt = "Interact";
>>>>>>> Stashed changes
    [SerializeField] private InteractableType interactionType = InteractableType.Generic;

    private bool isInvoking;

    public InteractableType InteractionType => interactionType;
<<<<<<< Updated upstream
    public bool IsInteractable => isActiveAndEnabled;
=======
    public string InteractionPrompt => interactionPrompt;
    public virtual bool IsInteractable => isActiveAndEnabled;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
    public virtual void Interact()
=======
    public void Interact()
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
