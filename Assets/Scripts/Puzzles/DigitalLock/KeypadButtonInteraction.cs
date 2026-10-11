
using UnityEngine;

public class KeypadButtonInteraction : MonoBehaviour, IInteractable
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

    private void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public bool IsInteractable => enabled && gameObject.activeInHierarchy;

    public void Interact()
    {
        Press();
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
