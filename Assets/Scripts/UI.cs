using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls textual interaction prompts and temporary on-screen feedback messages.
/// Compatible with standard UnityEngine.UI.Text and TextMeshProUGUI.
/// </summary>
public class UI : MonoBehaviour
{
    public static UI Instance { get; private set; }

    [Header("Text Displays")]
    [Tooltip("Legacy UnityEngine.UI.Text display.")]
    [SerializeField] private Text interactionText;

    [Tooltip("TextMeshPro display for sharp UI text.")]
    [SerializeField] private TextMeshProUGUI interactionTextTMP;

    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        // Auto-discover interaction text if unassigned
        if (interactionText == null && interactionTextTMP == null)
        {
            interactionTextTMP = GetComponentInChildren<TextMeshProUGUI>(true);
            if (interactionTextTMP == null)
            {
                interactionText = GetComponentInChildren<Text>(true);
            }
        }
    }

    public void SetInteractionText(string text)
    {
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

        ApplyText(text, true);
    }

    public void ClearInteractionText()
    {
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

        ApplyText("", false);
    }

    /// <summary>
    /// Displays a temporary feedback message (such as "Requires Key_Door1") for the specified duration.
    /// Calling this repeatedly cancels any ongoing timer and cleanly refreshes the display time.
    /// </summary>
    public void ShowInteractionFeedback(string message, float duration = 2.0f)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

        feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, duration));
    }

    private IEnumerator FeedbackRoutine(string message, float duration)
    {
        ApplyText(message, true);
        yield return new WaitForSeconds(duration);
        ApplyText("", false);
        feedbackCoroutine = null;
    }

    private void ApplyText(string text, bool active)
    {
        bool hasText = !string.IsNullOrEmpty(text);

        if (interactionTextTMP != null)
        {
            interactionTextTMP.text = text;
            interactionTextTMP.gameObject.SetActive(active && hasText);
        }

        if (interactionText != null)
        {
            interactionText.text = text;
            interactionText.gameObject.SetActive(active && hasText);
        }
    }
}
