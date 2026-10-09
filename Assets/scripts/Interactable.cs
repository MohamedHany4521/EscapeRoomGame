using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
public class Interactable : MonoBehaviour
{
    Outline outline;
    public string message;
    public UnityEvent onInteractation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline = GetComponent<Outline>();
        HideOutline();

    }

    public void ShowOutline()
    {
        outline.enabled = true;
    }

    public void HideOutline()
    {
        outline.enabled = false;
    }
    public void Interact()
    {
        onInteractation.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
