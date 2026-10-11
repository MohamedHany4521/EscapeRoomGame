using UnityEngine;

public interface IInteractable
{
    void Interact();
    void ShowOutline();
    void HideOutline();
    bool IsInteractable { get; }
}
