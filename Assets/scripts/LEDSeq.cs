using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LEDSeq : MonoBehaviour
{
    [SerializeField] private LEDButton[] ledButtons;
    [SerializeField] private float sequenceDelay = 0.6f;
    [SerializeField] private float ledOnDuration = 0.4f;
    [SerializeField] private float nextRoundDelay = 1f;
    [SerializeField] private int maxSequenceLength = 10;

    private List<int> sequence = new List<int>();
    private List<int> playerSequence = new List<int>();
    private bool isPlayingSequence = false;
    private bool isPlayerTurn = false;
    private int currentRound = 0;

    private void Start()
    {
        if (ledButtons == null || ledButtons.Length == 0)
        {
            Debug.LogError("LEDSeq: No LED buttons assigned!");
            return;
        }

        LEDButton.OnLEDButtonPressed += OnButtonPressed;
        Debug.Log("LED Sequence game ready! Press E on any button to start...");
    }

    private void OnDestroy()
    {
        LEDButton.OnLEDButtonPressed -= OnButtonPressed;
    }

    private void StartNewGame()
    {
        sequence.Clear();
        playerSequence.Clear();
        currentRound = 0;
        Debug.Log("Simon Says game started!");
        StartCoroutine(FlashAllLEDsAndStart());
    }

    private IEnumerator FlashAllLEDsAndStart()
    {
        // Flash all LEDs briefly
        foreach (var button in ledButtons)
        {
            button.TurnOn();
        }
        yield return new WaitForSeconds(0.3f);
        foreach (var button in ledButtons)
        {
            button.TurnOff();
        }
        yield return new WaitForSeconds(0.3f);

        // Start the first round
        NextRound();
    }

    private void NextRound()
    {
        currentRound++;
        playerSequence.Clear();

        if (sequence.Count < maxSequenceLength)
        {
            int randomIndex = Random.Range(0, ledButtons.Length);
            sequence.Add(randomIndex);
            Debug.Log($"Round {currentRound}: Added LED {randomIndex} to sequence");
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        isPlayingSequence = true;
        isPlayerTurn = false;

        yield return new WaitForSeconds(sequenceDelay);

        Debug.Log($"Playing sequence for round {currentRound}...");

        foreach (int ledIndex in sequence)
        {
            yield return StartCoroutine(PlayLED(ledIndex));
            yield return new WaitForSeconds(sequenceDelay);
        }

        isPlayingSequence = false;
        isPlayerTurn = true;
        Debug.Log($"Player's turn! Replicate {sequence.Count} LED(s).");
    }

    private IEnumerator PlayLED(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < ledButtons.Length && ledButtons[buttonIndex] != null)
        {
            ledButtons[buttonIndex].TurnOn();
            yield return new WaitForSeconds(ledOnDuration);
            ledButtons[buttonIndex].TurnOff();
        }
    }

    private void OnButtonPressed(int buttonIndex)
    {
        // Start game on first button press
        if (currentRound == 0)
        {
            Debug.Log($"Starting game with button {buttonIndex}!");
            StartNewGame();
            return;
        }

        if (!isPlayerTurn || isPlayingSequence)
            return;

        Debug.Log($"Player pressed LED {buttonIndex}");
        playerSequence.Add(buttonIndex);

        // Check if correct
        if (playerSequence[playerSequence.Count - 1] != sequence[playerSequence.Count - 1])
        {
            Debug.LogError($"Game Over! Wrong sequence. Restarting...");
            isPlayerTurn = false;
            StartCoroutine(RestartGameAfterDelay());
            return;
        }

        // Check if completed
        if (playerSequence.Count == sequence.Count)
        {
            isPlayerTurn = false;
            Debug.Log($"Round {currentRound} completed! Next round starting...");
            StartCoroutine(WaitAndNextRound());
        }
    }

    private IEnumerator WaitAndNextRound()
    {
        yield return new WaitForSeconds(nextRoundDelay);
        NextRound();
    }

    private IEnumerator RestartGameAfterDelay()
    {
        yield return new WaitForSeconds(nextRoundDelay);
        StartNewGame();
    }
}