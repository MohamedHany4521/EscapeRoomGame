using UnityEngine;

public interface IInteractable
{
    InteractableType InteractionType { get; }
    void Interact();
    void ShowOutline();
    void HideOutline();
    bool IsInteractable { get; }
}
