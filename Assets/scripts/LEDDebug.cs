using UnityEngine;

public class LEDDebug : MonoBehaviour
{
    [SerializeField] private LEDSeq ledSeq;

    private void Start()
    {
        if (ledSeq == null)
        {
            ledSeq = GetComponent<LEDSeq>();
        }

        Debug.Log("=== LED DEBUG INFO ===");

        LEDButton[] buttons = GetComponentsInChildren<LEDButton>();
        Debug.Log($"Found {buttons.Length} LEDButton components");

        for (int i = 0; i < buttons.Length; i++)
        {
            Debug.Log($"Button {i}: {buttons[i].gameObject.name}");
        }
    }
}
