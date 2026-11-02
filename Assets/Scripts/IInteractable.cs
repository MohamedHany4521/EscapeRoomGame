public interface IInteractable
{
    InteractableType InteractionType { get; }
    string InteractionPrompt { get; }
    bool IsInteractable { get; }
    void Interact();
    void ShowOutline();
    void HideOutline();
}
