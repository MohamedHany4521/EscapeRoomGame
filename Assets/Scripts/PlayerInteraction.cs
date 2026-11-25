using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float playerReach = 2f;
    [SerializeField] private GameObject normalCursor;
    [SerializeField] private GameObject adjustCursor;
    [SerializeField] private GameObject holdCursor;
    [SerializeField] private GameObject examineCursor;

    [Header("Cursor pop transition")]
    [SerializeField] private float cursorFadeInDuration = 0.35f;
    [SerializeField] private float cursorFadeOutDuration = 0.14f;
    [SerializeField, Range(0.1f, 1f)] private float cursorPopStartScale = 0.55f;
    [SerializeField, Range(1f, 1.5f)] private float cursorPopOvershoot = 1.12f;

    private IInteractable currentInteractable;
    private GameObject[] cursors;
    private CanvasGroup[] cursorGroups;
    private Vector3[] cursorBaseScales;
    private GameObject activeCursor;
    private GameObject requestedCursor;
    private Coroutine cursorTransition;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        cursors = new[]
        {
            normalCursor,
            adjustCursor,
            holdCursor,
            examineCursor
        };

        cursorGroups = new CanvasGroup[cursors.Length];
        cursorBaseScales = new Vector3[cursors.Length];

        for (int i = 0; i < cursors.Length; i++)
        {
            if (cursors[i] == null)
                continue;

            cursorGroups[i] = cursors[i].GetComponent<CanvasGroup>();

            if (cursorGroups[i] == null)
                cursorGroups[i] = cursors[i].AddComponent<CanvasGroup>();

            cursorBaseScales[i] = cursors[i].transform.localScale;
            cursorGroups[i].interactable = false;
            cursorGroups[i].blocksRaycasts = false;
            cursorGroups[i].alpha = 0f;
            cursors[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        SetCursorImmediately(normalCursor);
        activeCursor = normalCursor;
        requestedCursor = normalCursor;
    }

    private void OnDisable()
    {
        if (cursorTransition != null)
        {
            StopCoroutine(cursorTransition);
            cursorTransition = null;
        }

        SetCurrentInteractable(null);
        SetCursorImmediately(null);
        activeCursor = null;
        requestedCursor = null;
    }

    private void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            currentInteractable.Interact();
    }

    private void CheckInteraction()
    {
        if (playerCamera != null)
        {
            Ray ray = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
            {
                IInteractable target =
                    hit.collider.GetComponentInParent<IInteractable>();

                if (target != null && target.IsInteractable)
                {
                    SetCurrentInteractable(target);
                    ShowCursor(target.InteractionType);
                    return;
                }
            }
        }

        SetCurrentInteractable(null);
        ShowCursor(null);
    }

    private void SetCurrentInteractable(IInteractable target)
    {
        if (ReferenceEquals(currentInteractable, target))
            return;

        if (currentInteractable != null)
            currentInteractable.HideOutline();

        currentInteractable = target;

        if (currentInteractable != null)
            currentInteractable.ShowOutline();
    }

    private void ShowCursor(InteractableType? type)
    {
        GameObject cursor = normalCursor;

        if (type.HasValue)
        {
            switch (type.Value)
            {
                case InteractableType.Adjust:
                    cursor = adjustCursor;
                    break;
                case InteractableType.Hold:
                    cursor = holdCursor;
                    break;
                case InteractableType.Examine:
                    cursor = examineCursor;
                    break;
            }
        }

        if (requestedCursor == cursor)
            return;

        requestedCursor = cursor;

        if (cursorTransition == null)
            cursorTransition = StartCoroutine(TransitionCursors());
    }

    private IEnumerator TransitionCursors()
    {
        while (activeCursor != requestedCursor)
        {
            GameObject target = requestedCursor;
            int targetIndex = FindCursorIndex(target);

            if (targetIndex >= 0 && !cursors[targetIndex].activeSelf)
            {
                cursors[targetIndex].SetActive(true);
                cursorGroups[targetIndex].alpha = 0f;
                cursors[targetIndex].transform.localScale =
                    cursorBaseScales[targetIndex] * cursorPopStartScale;
            }

            float[] startAlphas = new float[cursors.Length];
            Vector3[] startScales = new Vector3[cursors.Length];

            for (int i = 0; i < cursors.Length; i++)
            {
                if (cursors[i] == null)
                    continue;

                startAlphas[i] = cursorGroups[i].alpha;
                startScales[i] = cursors[i].transform.localScale;
            }

            float fadeInDuration = Mathf.Max(0.01f, cursorFadeInDuration);
            float fadeOutDuration = Mathf.Max(0.01f, cursorFadeOutDuration);
            float duration = Mathf.Max(fadeInDuration, fadeOutDuration);
            float elapsed = 0f;

            while (elapsed < duration && requestedCursor == target)
            {
                elapsed += Time.unscaledDeltaTime;

                float incomingProgress = Mathf.Clamp01(elapsed / fadeInDuration);
                float outgoingProgress = Mathf.Clamp01(elapsed / fadeOutDuration);
                float incomingFade = Mathf.SmoothStep(0f, 1f, incomingProgress);
                float outgoingFade = Mathf.SmoothStep(0f, 1f, outgoingProgress);

                for (int i = 0; i < cursors.Length; i++)
                {
                    if (cursors[i] == null)
                        continue;

                    if (i == targetIndex)
                    {
                        cursorGroups[i].alpha = incomingFade;

                        float scale = GetPopScale(incomingProgress);
                        cursors[i].transform.localScale =
                            cursorBaseScales[i] * scale;
                    }
                    else
                    {
                        cursorGroups[i].alpha = Mathf.Lerp(
                            startAlphas[i],
                            0f,
                            outgoingFade);

                        cursors[i].transform.localScale = Vector3.Lerp(
                            startScales[i],
                            cursorBaseScales[i],
                            outgoingFade);
                    }
                }

                yield return null;
            }

            if (requestedCursor != target)
                continue;

            SetCursorImmediately(target);
            activeCursor = target;
        }

        SetCursorImmediately(activeCursor);
        cursorTransition = null;
    }

    private float GetPopScale(float progress)
    {
        const float overshootPoint = 0.7f;

        if (progress < overshootPoint)
        {
            float t = Mathf.SmoothStep(0f, 1f, progress / overshootPoint);
            return Mathf.Lerp(cursorPopStartScale, cursorPopOvershoot, t);
        }

        float settle = Mathf.SmoothStep(
            0f,
            1f,
            (progress - overshootPoint) / (1f - overshootPoint));

        return Mathf.Lerp(cursorPopOvershoot, 1f, settle);
    }

    private int FindCursorIndex(GameObject cursor)
    {
        if (cursor == null)
            return -1;

        for (int i = 0; i < cursors.Length; i++)
        {
            if (cursors[i] == cursor)
                return i;
        }

        return -1;
    }

    private void SetCursorImmediately(GameObject visibleCursor)
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            if (cursors[i] == null)
                continue;

            bool isVisible = cursors[i] == visibleCursor;
            cursorGroups[i].alpha = isVisible ? 1f : 0f;
            cursors[i].transform.localScale = cursorBaseScales[i];
            cursors[i].SetActive(isVisible);
        }
    }
}