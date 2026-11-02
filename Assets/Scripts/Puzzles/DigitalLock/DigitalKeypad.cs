using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DigitalKeypad : MonoBehaviour
{
    [Header("Password")]
    [SerializeField] private string correctPassword = "5567";

    [Header("Display")]
    [SerializeField] private TMP_Text displayText;

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color successTextColor = Color.green;
    [SerializeField] private Color errorTextColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip accessGrantedClip;
    [SerializeField] private AudioClip accessDeniedClip;

    [Header("Screen Visual Feedback")]
    [SerializeField] private Renderer screenPanelRenderer;
    [SerializeField]
    private Color screenNormalColor =
        new Color(1f, 0.35f, 0.05f);
    [SerializeField] private Color screenGrantedColor = Color.green;
    [SerializeField] private Color screenDeniedColor = Color.red;
    [SerializeField] private float screenIntensity = 2f;
    [SerializeField] private float resultDuration = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onPasswordCorrect;
    [SerializeField] private UnityEvent onPasswordWrong;

    private string currentInput = "";
    private bool isShowingResult;
    private Coroutine resultCoroutine;

    private void Awake()
    {
        SetTextColor(normalTextColor);
        SetScreenColor(screenNormalColor);
        UpdateDisplay();
    }

    public void PressNumber(string number)
    {
        if (isShowingResult || currentInput.Length >= 9)
            return;

        PlaySound(buttonClickClip);
        SetScreenColor(screenNormalColor);

        currentInput += number;
        UpdateDisplay();
    }

    public void PressEnter()
    {
        if (isShowingResult)
            return;

        bool isCorrect = currentInput == correctPassword;

        if (isCorrect)
        {
            Debug.Log("Correct password! Access granted.");
            onPasswordCorrect?.Invoke();
        }
        else
        {
            Debug.LogWarning("Wrong password!");
            onPasswordWrong?.Invoke();
        }

        if (resultCoroutine != null)
            StopCoroutine(resultCoroutine);

        resultCoroutine = StartCoroutine(
            ShowResultRoutine(isCorrect)
        );
    }

    public void PressClear()
    {
        if (isShowingResult)
            return;

        PlaySound(buttonClickClip);
        ClearInput();
        SetScreenColor(screenNormalColor);
    }

    private IEnumerator ShowResultRoutine(bool isCorrect)
    {
        isShowingResult = true;

        currentInput = "";

        if (isCorrect)
        {
            UpdateDisplay("Granted");
            SetTextColor(successTextColor);
            SetScreenColor(screenGrantedColor);
            PlaySound(accessGrantedClip);
        }
        else
        {
            UpdateDisplay("Denied");
            SetTextColor(errorTextColor);
            SetScreenColor(screenDeniedColor);
            PlaySound(accessDeniedClip);
        }

        yield return new WaitForSeconds(resultDuration);

        if (!isCorrect)
            UpdateDisplay("");

        SetScreenColor(screenNormalColor);
        SetTextColor(normalTextColor);
        isShowingResult = false;
        resultCoroutine = null;
    }

    private void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    private void UpdateDisplay(string message = null)
    {
        if (displayText == null)
            return;

        displayText.text = message ?? currentInput;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void SetScreenColor(Color color)
    {
        if (screenPanelRenderer == null)
            return;

        Material material = screenPanelRenderer.material;

        if (!material.HasProperty("_EmissionColor"))
            return;

        material.EnableKeyword("_EMISSION");
        material.SetColor(
            "_EmissionColor",
            color * screenIntensity
        );
    }
    
    private void SetTextColor(Color color)
    {
        if (displayText != null)
            displayText.color = color;
    }

}