using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
   [SerializeField] private Text interactionText;


    public void SetInteractionText(string text)
    {
        if (interactionText == null)
            return;

        interactionText.text = text;
        interactionText.gameObject.SetActive(true);
    }

    public void ClearInteractionText()
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(false);
    }
}
