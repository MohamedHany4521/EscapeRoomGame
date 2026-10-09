using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public static UI instance;

    private void Awake()
    {


        instance = this;

    }

    [SerializeField] private Text interactionText;


    public void SetInteractionText(string text)
    {
        interactionText.text = text ;
        interactionText.gameObject.SetActive(true);
    }

    public void ClearInteractionText()
    {
       
        interactionText.gameObject.SetActive(false);
    }
}


   


