<<<<<<< Updated upstream
using UnityEngine;

public interface IInteractable
{
=======
public interface IInteractable
{
    InteractableType InteractionType { get; }
>>>>>>> Stashed changes
    void Interact();
    void ShowOutline();
    void HideOutline();
    bool IsInteractable { get; }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
