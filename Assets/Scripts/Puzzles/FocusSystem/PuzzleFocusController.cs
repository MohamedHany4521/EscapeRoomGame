using System.Collections;
using FPC_CPP.Runtime;
using UnityEngine;

public class PuzzleFocusController : MonoBehaviour
{
    public static PuzzleFocusController Instance { get; private set; }

    public enum FocusState
    {
        Idle,
        TransitioningIn,
        Focused,
        TransitioningOut
    }

    [Header("Camera & Player References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerController_ControladorDelJugador playerController;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Canvas crosshairCanvas;

    [Header("Transition Settings")]
    [Tooltip("Default duration in seconds for camera transitions.")]
    [SerializeField, Min(0.05f)] private float defaultTransitionDuration = 0.5f;

    [Tooltip("Key used to exit puzzle focus.")]
    [SerializeField] private KeyCode exitKey = KeyCode.Escape;

    [Header("Interaction & Drag Settings")]
    [Tooltip("Movement distance in screen pixels before a pointer press is recognized as a drag instead of a click.")]
    [SerializeField, Min(1f)] private float dragThresholdPixels = 6f;

    [Tooltip("Maximum raycast distance for cursor clicks and drags on puzzle objects.")]
    [SerializeField, Min(1f)] private float maxRaycastDistance = 25f;

    [Tooltip("Layer mask for puzzle mouse interaction.")]
    [SerializeField] private LayerMask interactionLayerMask = ~0;

    private FocusState currentState = FocusState.Idle;
    private PuzzleInteractable currentPuzzle;

    // Saved transform properties for seamless restoration
    private Transform savedCameraParent;
    private Vector3 savedCameraLocalPosition;
    private Quaternion savedCameraLocalRotation;

    private Coroutine transitionCoroutine;

    // Mouse click vs drag tracking
    private Vector2 pointerDownPosition;
    private Vector2 previousMousePosition;
    private bool isDragging;
    private GameObject pointerDownTarget;
    private IPuzzleDraggable activeDraggable;

    // Hover outline tracking in focus mode
    private Outline currentHoveredOutline;

    public FocusState State => currentState;
    public bool IsFocused => currentState == FocusState.Focused;
    public bool IsTransitioning => currentState == FocusState.TransitioningIn || currentState == FocusState.TransitioningOut;
    public PuzzleInteractable CurrentPuzzle => currentPuzzle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureControllerExists()
    {
        if (Instance == null && FindFirstObjectByType<PuzzleFocusController>() == null)
        {
            GameObject managerObj = new GameObject("[PuzzleFocusController]");
            managerObj.AddComponent<PuzzleFocusController>();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveReferences();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ResolveReferences()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Camera[] allCams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera c in allCams)
            {
                if (c.CompareTag("MainCamera") || c.name.Contains("Main Camera"))
                {
                    playerCamera = c;
                    break;
                }
            }
            if (playerCamera == null && allCams.Length > 0)
                playerCamera = allCams[0];
        }

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController_ControladorDelJugador>();

        if (playerInteraction == null)
            playerInteraction = FindFirstObjectByType<PlayerInteraction>();

        if (crosshairCanvas == null)
        {
            GameObject crosshairObj = GameObject.Find("CrosshairCanvas");
            if (crosshairObj != null)
                crosshairCanvas = crosshairObj.GetComponent<Canvas>();
        }
    }

    private void Update()
    {
        if (currentState == FocusState.Focused)
        {
            if (Input.GetKeyDown(exitKey))
            {
                ExitFocus();
                return;
            }

            HandleFocusMouseInput();
        }
    }

    public void FocusOnPuzzle(PuzzleInteractable puzzle)
    {
        if (puzzle == null)
        {
            Debug.LogWarning("PuzzleFocusController: Cannot focus on null puzzle.", this);
            return;
        }

        Transform targetFocusPoint = puzzle.FocusPoint;
        if (targetFocusPoint == null)
        {
            Debug.LogWarning($"PuzzleFocusController: Puzzle {puzzle.name} has no FocusPoint assigned.", this);
            return;
        }

        if (currentState != FocusState.Idle)
            return;

        ResolveReferences();

        if (playerCamera == null)
        {
            Debug.LogError("PuzzleFocusController: No Player Camera found to perform focus transition.", this);
            return;
        }

        currentPuzzle = puzzle;
        currentState = FocusState.TransitioningIn;

        // Save original camera local pose relative to its current parent
        Transform camTransform = playerCamera.transform;
        savedCameraParent = camTransform.parent;
        savedCameraLocalPosition = camTransform.localPosition;
        savedCameraLocalRotation = camTransform.localRotation;

        // Disable player movement & camera control
        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Disable standard crosshair interaction
        if (playerInteraction != null)
            playerInteraction.enabled = false;

        // Hide crosshair canvas if present
        if (crosshairCanvas != null)
            crosshairCanvas.enabled = false;

        float duration = puzzle.CustomTransitionDuration > 0f ? puzzle.CustomTransitionDuration : defaultTransitionDuration;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionToFocusRoutine(targetFocusPoint, duration));
    }

    public void ExitFocus()
    {
        if (currentState != FocusState.Focused)
            return;

        currentState = FocusState.TransitioningOut;

        // Clean up any ongoing drag
        if (isDragging && activeDraggable != null)
        {
            activeDraggable.OnDragEnd(Input.mousePosition);
            activeDraggable = null;
        }

        ClearHoverOutline();
        isDragging = false;
        pointerDownTarget = null;

        // Lock & hide cursor during exit transition
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float duration = currentPuzzle != null && currentPuzzle.CustomTransitionDuration > 0f 
            ? currentPuzzle.CustomTransitionDuration 
            : defaultTransitionDuration;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionFromFocusRoutine(duration));
    }

    private IEnumerator TransitionToFocusRoutine(Transform targetFocusPoint, float duration)
    {
        Transform camTransform = playerCamera.transform;
        Vector3 startWorldPos = camTransform.position;
        Quaternion startWorldRot = camTransform.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Dynamic evaluation in case focus point moves
            Vector3 targetWorldPos = targetFocusPoint.position;
            Quaternion targetWorldRot = targetFocusPoint.rotation;

            if (currentPuzzle != null && currentPuzzle.FocusTarget != null && currentPuzzle.FocusTarget != targetFocusPoint)
            {
                Vector3 toTarget = currentPuzzle.FocusTarget.position - targetWorldPos;
                if (toTarget.sqrMagnitude > 0.001f)
                    targetWorldRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            }

            camTransform.position = Vector3.Lerp(startWorldPos, targetWorldPos, smoothT);
            camTransform.rotation = Quaternion.Slerp(startWorldRot, targetWorldRot, smoothT);

            yield return null;
        }

        Vector3 finalPos = targetFocusPoint.position;
        Quaternion finalRot = targetFocusPoint.rotation;
        if (currentPuzzle != null && currentPuzzle.FocusTarget != null && currentPuzzle.FocusTarget != targetFocusPoint)
        {
            Vector3 toTarget = currentPuzzle.FocusTarget.position - finalPos;
            if (toTarget.sqrMagnitude > 0.001f)
                finalRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        }

        camTransform.position = finalPos;
        camTransform.rotation = finalRot;

        // Unlock and display mouse cursor for puzzle interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentState = FocusState.Focused;
        transitionCoroutine = null;

        if (currentPuzzle != null)
            currentPuzzle.NotifyFocusEnter();
    }

    private IEnumerator TransitionFromFocusRoutine(float duration)
    {
        Transform camTransform = playerCamera.transform;
        Vector3 startWorldPos = camTransform.position;
        Quaternion startWorldRot = camTransform.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Calculate current world target of original local pose relative to parent
            Vector3 targetWorldPos;
            Quaternion targetWorldRot;

            if (savedCameraParent != null)
            {
                targetWorldPos = savedCameraParent.TransformPoint(savedCameraLocalPosition);
                targetWorldRot = savedCameraParent.rotation * savedCameraLocalRotation;
            }
            else
            {
                targetWorldPos = savedCameraLocalPosition;
                targetWorldRot = savedCameraLocalRotation;
            }

            camTransform.position = Vector3.Lerp(startWorldPos, targetWorldPos, smoothT);
            camTransform.rotation = Quaternion.Slerp(startWorldRot, targetWorldRot, smoothT);

            yield return null;
        }

        // Snap back to exact saved local transform
        camTransform.localPosition = savedCameraLocalPosition;
        camTransform.localRotation = savedCameraLocalRotation;

        // Restore player controls and interaction
        if (playerController != null)
            playerController.enabled = true;

        if (playerInteraction != null)
            playerInteraction.enabled = true;

        if (crosshairCanvas != null)
            crosshairCanvas.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentState = FocusState.Idle;
        transitionCoroutine = null;

        if (currentPuzzle != null)
        {
            currentPuzzle.NotifyFocusExit();
            currentPuzzle = null;
        }
    }

    private void HandleFocusMouseInput()
    {
        if (playerCamera == null)
            return;

        Vector2 currentMousePos = Input.mousePosition;

        // Hover outline detection
        Ray hoverRay = playerCamera.ScreenPointToRay(currentMousePos);
        if (Physics.Raycast(hoverRay, out RaycastHit hoverHit, maxRaycastDistance, interactionLayerMask))
        {
            Outline outline = hoverHit.collider.GetComponentInParent<Outline>();
            if (outline != currentHoveredOutline)
            {
                ClearHoverOutline();
                currentHoveredOutline = outline;
                if (currentHoveredOutline != null && !isDragging)
                    currentHoveredOutline.enabled = true;
            }
        }
        else
        {
            ClearHoverOutline();
        }

        // Pointer Down
        if (Input.GetMouseButtonDown(0))
        {
            pointerDownPosition = currentMousePos;
            previousMousePosition = currentMousePos;
            isDragging = false;
            activeDraggable = null;
            pointerDownTarget = null;

            Ray ray = playerCamera.ScreenPointToRay(currentMousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, interactionLayerMask))
            {
                pointerDownTarget = hit.collider.gameObject;
            }
        }

        // Pointer Held (check drag threshold)
        if (Input.GetMouseButton(0) && pointerDownTarget != null)
        {
            Vector2 deltaSincePrevious = currentMousePos - previousMousePosition;

            if (!isDragging)
            {
                float distMoved = Vector2.Distance(currentMousePos, pointerDownPosition);
                if (distMoved >= dragThresholdPixels)
                {
                    IPuzzleDraggable draggable = pointerDownTarget.GetComponentInParent<IPuzzleDraggable>();
                    if (draggable != null)
                    {
                        isDragging = true;
                        activeDraggable = draggable;
                        activeDraggable.OnDragStart(pointerDownPosition);
                    }
                }
            }

            if (isDragging && activeDraggable != null)
            {
                activeDraggable.OnDrag(currentMousePos, deltaSincePrevious);
            }

            previousMousePosition = currentMousePos;
        }

        // Pointer Up
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                if (activeDraggable != null)
                {
                    activeDraggable.OnDragEnd(currentMousePos);
                    activeDraggable = null;
                }
                isDragging = false;
            }
            else
            {
                // Dispatched as a clean click
                if (pointerDownTarget != null)
                {
                    DispatchClick(pointerDownTarget);
                }
            }

            pointerDownTarget = null;
        }
    }

    private void DispatchClick(GameObject target)
    {
        if (target == null) return;

        // 1. IPuzzleClickable
        IPuzzleClickable clickable = target.GetComponentInParent<IPuzzleClickable>();
        if (clickable != null)
        {
            clickable.OnPuzzleClick();
            return;
        }

        // 2. KeypadButtonInteraction
        KeypadButtonInteraction keypadBtn = target.GetComponentInParent<KeypadButtonInteraction>();
        if (keypadBtn != null)
        {
            keypadBtn.Press();
            return;
        }

        // 3. IInteractable fallback
        IInteractable interactable = target.GetComponentInParent<IInteractable>();
        if (interactable != null && interactable.IsInteractable)
        {
            interactable.Interact();
        }
    }

    private void ClearHoverOutline()
    {
        if (currentHoveredOutline != null)
        {
            currentHoveredOutline.enabled = false;
            currentHoveredOutline = null;
        }
    }

    private void OnDisable()
    {
        ClearHoverOutline();

        if (currentState != FocusState.Idle)
        {
            if (playerCamera != null && savedCameraParent != null)
            {
                playerCamera.transform.localPosition = savedCameraLocalPosition;
                playerCamera.transform.localRotation = savedCameraLocalRotation;
            }

            if (playerController != null)
                playerController.enabled = true;

            if (playerInteraction != null)
                playerInteraction.enabled = true;

            if (crosshairCanvas != null)
                crosshairCanvas.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            currentState = FocusState.Idle;
            currentPuzzle = null;
        }
    }
}
