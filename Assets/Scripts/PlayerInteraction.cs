using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float playerReach = 3.5f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool allowMouseClick = true;

    [Header("Dynamic Cursor UI")]
    [SerializeField] private GameObject normalCursor;
    [SerializeField] private GameObject adjustCursor;
    [SerializeField] private GameObject holdCursor;
    [SerializeField] private GameObject examineCursor;

    [Header("Cursor Pop Transition")]
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
    private bool hasCursors;

    private void Awake()
    {
        PlayerInteraction[] instances = GetComponents<PlayerInteraction>();
        if (instances.Length > 1 && instances[0] != this)
        {
            enabled = false;
            return;
        }

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
            playerCamera = GetComponent<Camera>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            Debug.LogError("PlayerInteraction: No player camera assigned or found.", this);

        if (normalCursor == null || adjustCursor == null || holdCursor == null || examineCursor == null)
        {
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                if (normalCursor == null)
                {
                    Transform t = uiRoot.transform.Find("NormalCursor");
                    if (t != null) normalCursor = t.gameObject;
                }

                if (adjustCursor == null)
                {
                    Transform t = uiRoot.transform.Find("Adjust");
                    if (t != null) adjustCursor = t.gameObject;
                }

                if (holdCursor == null)
                {
                    Transform t = uiRoot.transform.Find("Hold");
                    if (t != null) holdCursor = t.gameObject;
                }

                if (examineCursor == null)
                {
                    Transform t = uiRoot.transform.Find("Examine");
                    if (t != null) examineCursor = t.gameObject;
                }
            }
        }

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

            hasCursors = true;
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
        if (hasCursors)
        {
            SetCursorImmediately(normalCursor);
            activeCursor = normalCursor;
            requestedCursor = normalCursor;
        }
    }

    private void OnDisable()
    {
        if (cursorTransition != null)
        {
            StopCoroutine(cursorTransition);
            cursorTransition = null;
        }

        SetCurrentInteractable(null);

        if (hasCursors)
        {
            SetCursorImmediately(null);
            activeCursor = null;
            requestedCursor = null;
        }
    }

    private void Update()
    {
        CheckInteraction();

        bool interactTriggered = Input.GetKeyDown(interactionKey) || (allowMouseClick && Input.GetMouseButtonDown(0));

        if (interactTriggered &&
            currentInteractable != null &&
            currentInteractable.IsInteractable)
        {
            currentInteractable.Interact();
        }
    }

    private void CheckInteraction()
    {
        if (playerCamera == null || !playerCamera.enabled)
        {
            SetCurrentInteractable(null);
            ShowCursor(null);
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        IInteractable detectedInteractable = null;

        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            MonoBehaviour[] behaviours =
                hit.collider.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable &&
                    interactable.IsInteractable)
                {
                    detectedInteractable = interactable;
                    break;
                }
            }
        }

        if (detectedInteractable != null)
        {
            SetCurrentInteractable(detectedInteractable);
            ShowCursor(detectedInteractable.InteractionType);
        }
        else
        {
            SetCurrentInteractable(null);
            ShowCursor(null);
        }
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
        if (!hasCursors)
            return;

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

                case InteractableType.Generic:
                    cursor = normalCursor;
                    break;
            }
        }

        if (cursor == null)
            cursor = normalCursor;

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

            if (targetIndex >= 0 && cursors[targetIndex] != null && !cursors[targetIndex].activeSelf)
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
