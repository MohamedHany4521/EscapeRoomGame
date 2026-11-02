using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class RoomRestartButton : MonoBehaviour
{
    [SerializeField] private Transform buttonTop;
    [SerializeField] private TMP_Text label;
    [SerializeField, Min(0f)] private float pressDistance = 0.03f;
    [SerializeField, Min(0.01f)] private float pressDuration = 0.08f;

    private Vector3 buttonTopStartPosition;
    private Coroutine restartCoroutine;
    private int roomSceneBuildIndex;
    private bool restartQueued;

    private void Awake()
    {
        if (buttonTop == null)
        {
            Debug.LogError("RoomRestartButton: Assign the movable button top.", this);
        }
        else
        {
            buttonTopStartPosition = buttonTop.localPosition;
        }

        if (label != null)
            label.text = "Restart Button";
        else
            Debug.LogWarning("RoomRestartButton: Assign a 3D text label to display 'Restart Button'.", this);

        roomSceneBuildIndex = gameObject.scene.buildIndex;
        if (roomSceneBuildIndex < 0)
        {
            Debug.LogError(
                $"RoomRestartButton: Scene '{gameObject.scene.name}' must be added to Build Settings to restart it.",
                this);
        }
    }

    private void OnDisable()
    {
        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine);
            restartCoroutine = null;
        }

        if (buttonTop != null)
            buttonTop.localPosition = buttonTopStartPosition;

        restartQueued = false;
    }

    public void Interact()
    {
        if (restartQueued || !isActiveAndEnabled || buttonTop == null || roomSceneBuildIndex < 0)
            return;

        restartQueued = true;
        restartCoroutine = StartCoroutine(PressAndRestartRoom());
    }

    private IEnumerator PressAndRestartRoom()
    {
        Vector3 pressedPosition = buttonTopStartPosition + Vector3.down * pressDistance;
        yield return AnimateButton(buttonTopStartPosition, pressedPosition);
        yield return AnimateButton(pressedPosition, buttonTopStartPosition);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(roomSceneBuildIndex, LoadSceneMode.Single);
        if (loadOperation == null)
        {
            Debug.LogError("RoomRestartButton: Unity could not start loading the current room.", this);
            restartQueued = false;
            restartCoroutine = null;
        }
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
