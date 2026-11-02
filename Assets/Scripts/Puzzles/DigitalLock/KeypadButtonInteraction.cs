
using UnityEngine;

public class KeypadButtonInteraction : MonoBehaviour, IInteractable, IPuzzleClickable
{
    public enum ButtonType
    {
        Number,
        Enter,
        Clear
    }

    [Header("Button Settings")]
    [SerializeField] private ButtonType buttonType = ButtonType.Number;
    [SerializeField] private string number;

    [Header("Keypad Reference")]
    [SerializeField] private DigitalKeypad keypad;

    [Header("Outline")]
    [SerializeField] private Outline outline;
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField] private float outlineWidth = 2f;

    private void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
        }

        if (outline != null)
            outline.enabled = false;
    }

    public bool IsInteractable => enabled && gameObject.activeInHierarchy;

    public void Interact()
    {
        // If part of a puzzle with PuzzleInteractable and player is not already in focus mode,
        // forward the first-person interaction (E key) to the puzzle focus system.
        PuzzleInteractable puzzle = GetComponentInParent<PuzzleInteractable>();
        if (puzzle != null && puzzle.IsInteractable && PuzzleFocusController.Instance != null && !PuzzleFocusController.Instance.IsFocused)
        {
            puzzle.Interact();
            return;
        }

        Press();
    }

    public void OnPuzzleClick()
    {
        Press();
    }

    public void ShowOutline()
    {
        PuzzleInteractable puzzle = GetComponentInParent<PuzzleInteractable>();
        if (puzzle != null && puzzle.IsInteractable && PuzzleFocusController.Instance != null && !PuzzleFocusController.Instance.IsFocused)
        {
            puzzle.ShowOutline();
            return;
        }

        if (outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        PuzzleInteractable puzzle = GetComponentInParent<PuzzleInteractable>();
        if (puzzle != null)
        {
            puzzle.HideOutline();
        }

        if (outline != null)
            outline.enabled = false;
    }

    public void Press()
    {
        if (keypad == null)
        {
            Debug.LogWarning($"No DigitalKeypad assigned to {gameObject.name}", this);
            return;
        }

        switch (buttonType)
        {
            case ButtonType.Number:
                keypad.PressNumber(number);
                break;

            case ButtonType.Enter:
                keypad.PressEnter();
                break;

            case ButtonType.Clear:
                keypad.PressClear();
                break;
        }
    }
}
