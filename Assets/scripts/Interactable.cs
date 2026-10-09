using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline outline;
    [SerializeField] private UnityEvent onInteract;

    private void OnEnable()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public void ShowOutline()
    {
        if (outline == null)
        {
            Debug.LogWarning($"No Outline component assigned or found on {gameObject.name}");
            return;
        }
        outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public virtual void Interact()
    {
        Debug.Log($"Interacted with: {gameObject.name}");
        onInteract?.Invoke();
    }

    public bool IsInteractable => gameObject.activeSelf && enabled;
}
