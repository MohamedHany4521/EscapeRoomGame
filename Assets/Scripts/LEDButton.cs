using System;
using System.Collections;
using UnityEngine;

public class LEDButton : MonoBehaviour
{
    [SerializeField] private GameObject ledObject;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private int buttonIndex;

    private Renderer ledRenderer;
    private Coroutine feedbackCoroutine;
    private bool isOn;

    public static event Action<int> OnLEDButtonPressed;

    public bool IsOn => isOn;
    public int ButtonIndex => buttonIndex;

    private void Awake()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        if (ledObject == null)
        {
            Debug.LogError($"LED Button {buttonIndex}: Assign an LED object.", this);
            return;
        }

        ledRenderer = ledObject.GetComponent<Renderer>();

        if (ledRenderer == null)
        {
            Debug.LogError($"LED Button {buttonIndex}: The LED object needs a Renderer.", this);
            return;
        }

        if (offMaterial == null || onMaterial == null)
        {
            Debug.LogWarning(
                $"LED Button {buttonIndex}: Assign both On Material and Off Material.",
                this);
        }
    }

    private void OnEnable()
    {
        TurnOff();
    }

    private void OnDisable()
    {
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

        TurnOff();
    }

    public void Interact()
    {
        OnLEDButtonPressed?.Invoke(buttonIndex);

        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(PlayFeedback());
    }

    public void TurnOn()
    {
        SetLEDMaterial(onMaterial);
        isOn = true;
    }

    public void TurnOff()
    {
        SetLEDMaterial(offMaterial);
        isOn = false;
    }

    private void SetLEDMaterial(Material material)
    {
        if (ledRenderer != null && material != null)
            ledRenderer.sharedMaterial = material;
    }

    private IEnumerator PlayFeedback()
    {
        TurnOn();
        yield return new WaitForSeconds(0.4f);
        TurnOff();
        feedbackCoroutine = null;
    }
}
