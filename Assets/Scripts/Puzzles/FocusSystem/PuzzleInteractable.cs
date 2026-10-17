using UnityEngine;
using UnityEngine.Events;

public class PuzzleInteractable : MonoBehaviour, IInteractable
{
    [Header("Focus Configuration")]
    [Tooltip("Transform defining the camera position and orientation when focused on this puzzle.")]
    [SerializeField] private Transform focusPoint;

    [Tooltip("Optional transform for the camera to look at while focused. If set, camera will look toward this target. Defaults to this object if null.")]
    [SerializeField] private Transform focusTarget;

    [Tooltip("Friendly display name of the puzzle.")]
    [SerializeField] private string puzzleName = "Puzzle";

    [Tooltip("Duration in seconds for camera transition to/from this puzzle (0 = use controller default).")]
    [SerializeField, Min(0f)] private float customTransitionDuration = 0f;

    [Header("Visual Feedback")]
    [SerializeField] private Outline outline;

    [Header("Events")]
    [SerializeField] private UnityEvent onFocusEnter;
    [SerializeField] private UnityEvent onFocusExit;

    public Transform FocusPoint
    {
        get
        {
            if (focusPoint == null)
                EnsureFocusPoint();
            return focusPoint;
        }
        set => focusPoint = value;
    }

    public Transform FocusTarget
    {
        get => focusTarget;
        set => focusTarget = value;
    }

    public string PuzzleName
    {
        get => puzzleName;
        set => puzzleName = value;
    }

    public float CustomTransitionDuration
    {
        get => customTransitionDuration;
        set => customTransitionDuration = Mathf.Max(0f, value);
    }

    public bool IsInteractable => enabled && gameObject.activeInHierarchy;

    private void Awake()
    {
        EnsureFocusPoint();

        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    private void EnsureFocusPoint()
    {
        if (focusPoint != null)
            return;

        // 1. Try finding child named "FocusPoint"
        Transform found = transform.Find("FocusPoint");
        if (found == null)
        {
            // Search all children for any object with focus in the name
            foreach (Transform child in transform)
            {
                if (child.name.ToLower().Contains("focus"))
                {
                    found = child;
                    break;
                }
            }
        }

        if (found != null)
        {
            focusPoint = found;
            return;
        }

        // 2. Auto-generate default FocusPoint in front of the puzzle
        GameObject fpObj = new GameObject("FocusPoint");
        
        Transform targetT = focusTarget != null ? focusTarget : transform;
        Vector3 targetCenter = targetT.position;
        Collider col = targetT.GetComponentInChildren<Collider>();
        if (col != null)
            targetCenter = col.bounds.center;

        // Place 0.55m in front of puzzle along sensible normal toward camera
        Camera mainCam = Camera.main;
        Vector3 toCam = mainCam != null ? (mainCam.transform.position - targetCenter).normalized : -targetT.forward;
        if (toCam == Vector3.zero)
            toCam = Vector3.forward;

        Vector3 worldPos = targetCenter + toCam * 0.55f;
        Quaternion worldRot = Quaternion.LookRotation((targetCenter - worldPos).normalized, Vector3.up);

        fpObj.transform.SetPositionAndRotation(worldPos, worldRot);
        fpObj.transform.SetParent(targetT, true);

        focusPoint = fpObj.transform;
    }

    public void ShowOutline()
    {
        if (outline != null && IsInteractable)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void Interact()
    {
        if (!IsInteractable)
            return;

        HideOutline();

        if (PuzzleFocusController.Instance != null)
        {
            PuzzleFocusController.Instance.FocusOnPuzzle(this);
        }
        else
        {
            Debug.LogWarning($"PuzzleInteractable on {gameObject.name}: No PuzzleFocusController found in scene.", this);
        }
    }

    public void NotifyFocusEnter()
    {
        onFocusEnter?.Invoke();
    }

    public void NotifyFocusExit()
    {
        onFocusExit?.Invoke();
    }

    public void SetFocusPoint(Transform point)
    {
        focusPoint = point;
    }

    public void SetFocusTarget(Transform target)
    {
        focusTarget = target;
    }
}
