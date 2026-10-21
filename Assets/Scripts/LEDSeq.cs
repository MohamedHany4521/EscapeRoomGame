using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LEDSeq : MonoBehaviour
{
    [SerializeField] private LEDButton[] ledButtons;
    [SerializeField] private float sequenceDelay = 0.6f;
    [SerializeField] private float ledOnDuration = 0.4f;
    [SerializeField] private float nextRoundDelay = 1f;
    [SerializeField] private int maxSequenceLength = 6;

    [Header("Random Code")]
    [SerializeField] private int minRandomCode = 0;
    [SerializeField] private int maxRandomCode = 9;
    [SerializeField] private TextMeshProUGUI[] randomCodeDigits = new TextMeshProUGUI[4];

    private readonly List<int> sequence = new List<int>();
    private readonly List<int> playerSequence = new List<int>();

    private int[] randomCode = new int[0];
    private int currentRound;
    private bool isPlayingSequence;
    private bool isPlayerTurn;
    private bool isConfigured;

    private void Awake()
    {
        isConfigured = ledButtons != null && ledButtons.Length > 0;

        if (!isConfigured)
        {
            Debug.LogError("LEDSeq: Assign at least one LED button.", this);
            return;
        }

        for (int i = 0; i < ledButtons.Length; i++)
        {
            if (ledButtons[i] == null)
            {
                Debug.LogError($"LEDSeq: LED button at array position {i} is not assigned.", this);
                isConfigured = false;
                continue;
            }

            for (int j = 0; j < i; j++)
            {
                if (ledButtons[j] != null &&
                    ledButtons[j].ButtonIndex == ledButtons[i].ButtonIndex)
                {
                    Debug.LogError(
                        $"LEDSeq: Button index {ledButtons[i].ButtonIndex} is assigned more than once.",
                        this);
                    isConfigured = false;
                }
            }
        }
    }

    private void OnEnable()
    {
        LEDButton.OnLEDButtonPressed += OnButtonPressed;
    }

    private void OnDisable()
    {
        LEDButton.OnLEDButtonPressed -= OnButtonPressed;
    }

    private void Start()
    {
        if (isConfigured)
            Debug.Log("LED sequence game ready. Press E on a button to start.");
    }

    private void StartNewGame()
    {
        sequence.Clear();
        playerSequence.Clear();
        currentRound = 0;

        Debug.Log("Simon Says game started!");
        StartCoroutine(NextRound());
    }

    private IEnumerator NextRound()
    {
        currentRound++;

        if (currentRound > maxSequenceLength)
        {
            Debug.Log($"You won! All {maxSequenceLength} rounds completed.");
            currentRound = 0;
            ShowRandomCode();
            yield break;
        }

        int randomLEDIndex = Random.Range(0, ledButtons.Length);
        sequence.Add(randomLEDIndex);

        Debug.Log($"Round {currentRound}: Added LED {randomLEDIndex}.");
        yield return PlaySequenceAndWaitForPlayer();
    }

    private IEnumerator PlaySequenceAndWaitForPlayer()
    {
        while (true)
        {
            yield return PlaySequence();

            playerSequence.Clear();
            isPlayerTurn = true;

            Debug.Log($"Your turn! Repeat {sequence.Count} LED(s).");

            while (playerSequence.Count < sequence.Count && isPlayerTurn)
                yield return new WaitForSeconds(0.1f);

            isPlayerTurn = false;

            if (IsPlayerSequenceCorrect())
            {
                yield return new WaitForSeconds(nextRoundDelay);
                yield return NextRound();
                yield break;
            }

            yield return new WaitForSeconds(nextRoundDelay);
        }
    }

    private bool IsPlayerSequenceCorrect()
    {
        if (playerSequence.Count != sequence.Count)
            return false;

        for (int i = 0; i < sequence.Count; i++)
        {
            if (playerSequence[i] != sequence[i])
                return false;
        }

        return true;
    }

    private IEnumerator PlaySequence()
    {
        isPlayingSequence = true;
        isPlayerTurn = false;

        yield return new WaitForSeconds(sequenceDelay);

        foreach (int ledIndex in sequence)
        {
            yield return PlayLED(ledIndex);
            yield return new WaitForSeconds(sequenceDelay);
        }

        isPlayingSequence = false;
    }

    private IEnumerator PlayLED(int ledIndex)
    {
        if (ledIndex < 0 || ledIndex >= ledButtons.Length || ledButtons[ledIndex] == null)
            yield break;

        ledButtons[ledIndex].TurnOn();
        yield return new WaitForSeconds(ledOnDuration);
        ledButtons[ledIndex].TurnOff();
    }

    private void OnButtonPressed(int buttonIndex)
    {
        if (!isConfigured)
            return;

        int ledArrayIndex = FindLEDArrayIndex(buttonIndex);

        if (ledArrayIndex < 0)
        {
            Debug.LogWarning($"LEDSeq: No LED button has button index {buttonIndex}.", this);
            return;
        }

        if (currentRound == 0)
        {
            Debug.Log($"Starting game with button {buttonIndex}.");
            StartNewGame();
            return;
        }

        if (!isPlayerTurn ||
            isPlayingSequence ||
            playerSequence.Count >= sequence.Count)
        {
            return;
        }

        playerSequence.Add(ledArrayIndex);
    }

    private int FindLEDArrayIndex(int buttonIndex)
    {
        for (int i = 0; i < ledButtons.Length; i++)
        {
            if (ledButtons[i].ButtonIndex == buttonIndex)
                return i;
        }

        return -1;
    }

    private void ShowRandomCode()
    {
        if (randomCodeDigits == null || randomCodeDigits.Length == 0)
        {
            Debug.LogError("LEDSeq: Assign the random-code text fields.", this);
            return;
        }

        if (maxRandomCode < minRandomCode)
        {
            Debug.LogError("LEDSeq: Maximum random-code digit must be at least the minimum.", this);
            return;
        }

        randomCode = new int[randomCodeDigits.Length];

        for (int i = 0; i < randomCodeDigits.Length; i++)
        {
            randomCode[i] = Random.Range(minRandomCode, maxRandomCode + 1);

            if (randomCodeDigits[i] == null)
            {
                Debug.LogError($"LEDSeq: Random-code text field {i} is not assigned.", this);
                continue;
            }

            randomCodeDigits[i].text = randomCode[i].ToString();
            randomCodeDigits[i].gameObject.SetActive(true);
        }
    }
}
