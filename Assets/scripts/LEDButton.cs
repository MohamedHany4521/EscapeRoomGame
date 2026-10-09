using UnityEngine;
using System;

public class LEDButton : MonoBehaviour
{
    [SerializeField] private GameObject ledObject;
    [SerializeField] private Material ledOnMaterial;
    [SerializeField] private Material ledOffMaterial;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private int buttonIndex = 0;

    private Collider buttonCollider;
    private Renderer ledRenderer;
    private Outline outline;
    private bool isOn = false;

    public static event Action<int> OnLEDButtonPressed; // Event system

    private void OnEnable()
    {
        buttonCollider = GetComponent<Collider>();
        if (buttonCollider == null)
            buttonCollider = gameObject.AddComponent<BoxCollider>();

        if (ledObject != null)
            ledRenderer = ledObject.GetComponent<Renderer>();

        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        TurnOff();
    }

    public void Interact()
    {
        Debug.Log($"🎮 LED Button {buttonIndex} pressed!");
        OnLEDButtonPressed?.Invoke(buttonIndex); // Fire event
        StartCoroutine(PlayLEDFeedback());
    }


    public bool IsInteractable => gameObject.activeSelf;

    private System.Collections.IEnumerator PlayLEDFeedback()
    {
        TurnOn();
        yield return new WaitForSeconds(0.4f);
        TurnOff();
    }

    public void TurnOn()
    {
        if (ledOnMaterial != null && ledRenderer != null)
        {
            Material[] materials = ledRenderer.materials;
            if (materialIndex < materials.Length)
            {
                materials[materialIndex] = ledOnMaterial;
                ledRenderer.materials = materials;
            }
        }
        isOn = true;
    }

    public void TurnOff()
    {
        if (ledOffMaterial != null && ledRenderer != null)
        {
            Material[] materials = ledRenderer.materials;
            if (materialIndex < materials.Length)
            {
                materials[materialIndex] = ledOffMaterial;
                ledRenderer.materials = materials;
            }
        }
        isOn = false;
    }

    public bool IsOn => isOn;
    public int ButtonIndex => buttonIndex;
}