<<<<<<< Updated upstream
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
=======
using UnityEngine;
using System;

public class LEDButton : MonoBehaviour 
{
    [SerializeField] private GameObject ledObject;
    [SerializeField] private float emissionIntensity = 2f;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private int buttonIndex = 0;

    private Collider buttonCollider;
    private Renderer ledRenderer;
    private Outline outline;
    private bool isOn;
    private Material materialInstance;
    private Color baseEmissionColor = Color.white;
    public InteractableType InteractionType;
    public static event Action<int> OnLEDButtonPressed;

    private void OnEnable()
    {
        buttonCollider = GetComponent<Collider>();
        if (buttonCollider == null)
            buttonCollider = gameObject.AddComponent<BoxCollider>();

        if (ledObject != null)
        {
            ledRenderer = ledObject.GetComponent<Renderer>();

            if (ledRenderer != null)
            {
                Material[] materials = ledRenderer.materials;

                if (materialIndex >= 0 && materialIndex < materials.Length)
                {
                    materialInstance = Instantiate(materials[materialIndex]);

                    if (materialInstance.HasProperty("_EmissionMap"))
                    {
                        Texture emissionMap = materialInstance.GetTexture("_EmissionMap");

                        if (emissionMap == null)
                        {
                            if (materialInstance.HasProperty("_BaseMap"))
                                emissionMap = materialInstance.GetTexture("_BaseMap");
                            else if (materialInstance.HasProperty("_MainTex"))
                                emissionMap = materialInstance.GetTexture("_MainTex");

                            if (emissionMap != null)
                                materialInstance.SetTexture("_EmissionMap", emissionMap);
                        }

                        if (emissionMap == null)
                        {
                            Debug.LogWarning(
                                $"LED Button {buttonIndex}: No emission or base texture found. Assign an emission map to the material.");
                        }
                    }

                    if (materialInstance.HasProperty("_EmissionColor"))
                    {
                        baseEmissionColor = materialInstance.GetColor("_EmissionColor");

                        if (baseEmissionColor.maxColorComponent <= 0f)
                            baseEmissionColor = Color.white;
                    }

                    materialInstance.EnableKeyword("_EMISSION");
                    materialInstance.globalIlluminationFlags =
                        MaterialGlobalIlluminationFlags.BakedEmissive;

                    materials[materialIndex] = materialInstance;
                    ledRenderer.materials = materials;
                }
                else
                {
                    Debug.LogError(
                        $"LED Button {buttonIndex}: Material index {materialIndex} is out of range.");
                }
            }
        }

        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        TurnOff();
    }

    public void Interact()
    {
        Debug.Log($"LED Button {buttonIndex} pressed!");
        OnLEDButtonPressed?.Invoke(buttonIndex);
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
        if (materialInstance != null && materialInstance.HasProperty("_EmissionColor"))
        {
            materialInstance.SetColor(
                "_EmissionColor",
                baseEmissionColor * emissionIntensity);
        }

        isOn = true;
    }

    public void TurnOff()
    {
        if (materialInstance != null && materialInstance.HasProperty("_EmissionColor"))
            materialInstance.SetColor("_EmissionColor", Color.black);

        isOn = false;
    }

    public bool IsOn => isOn;
    public int ButtonIndex => buttonIndex;
>>>>>>> Stashed changes
}