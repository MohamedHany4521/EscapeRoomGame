using UnityEngine;

public class KeypadButtonInteraction : MonoBehaviour
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

    public void Press()
    {
        if (keypad == null)
            return;

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