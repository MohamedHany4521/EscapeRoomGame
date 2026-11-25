using System;
using System.Collections;
using UnityEngine;

public class LEDButton : MonoBehaviour
{
    [SerializeField] private GameObject ledObject;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private int buttonIndex;
    [SerializeField] private Transform buttonTop;
    [SerializeField] private Vector3 pressDirection = Vector3.back;
    private Vector3 buttonTopStartPosition;
    [SerializeField] private float pressDistance = 0.03f;
    [SerializeField] private float pressDuration = 0.08f;
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

        if (buttonTop == null)
        {
            Debug.Log("Button: Assign the movable button top.", this);
        }
        else
        {
            buttonTopStartPosition = buttonTop.localPosition;
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

        if (buttonTop != null)
            buttonTop.localPosition = buttonTopStartPosition;
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
        Vector3 pressedPosition = buttonTopStartPosition + pressDirection * pressDistance;
        yield return AnimateButton(buttonTopStartPosition, pressedPosition);
        yield return AnimateButton(pressedPosition, buttonTopStartPosition);
        TurnOn();
        yield return new WaitForSeconds(0.4f);
        TurnOff();
        feedbackCoroutine = null;
    }

    private IEnumerator AnimateButton(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, pressDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            buttonTop.localPosition = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }

        buttonTop.localPosition = end;
    }
}
