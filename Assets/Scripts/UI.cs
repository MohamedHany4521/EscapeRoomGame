using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public static UI instance;

    private void Awake()
    {
        instance = this;
        ClearInteractionText();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }


    [SerializeField] private Text interactionText;


    public void SetInteractionText(string text)
    {
        if (interactionText == null)
        {
            Debug.LogWarning("UI: Assign the interaction text component.", this);
            return;
        }

        if (interactionText.text != text)
            interactionText.text = text;

        if (!interactionText.gameObject.activeSelf)
            interactionText.gameObject.SetActive(true);
    }

    public void ClearInteractionText()
    {
        if (interactionText != null && interactionText.gameObject.activeSelf)
            interactionText.gameObject.SetActive(false);
    }
}