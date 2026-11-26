using UnityEngine;
using UnityEngine.Events;

public class PuzzleDraggable : MonoBehaviour, IPuzzleDraggable
{
    public enum DragMode
    {
        CustomEvent,
        RotateTransform,
        TranslateTransform
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    [Header("Drag Configuration")]
    [SerializeField] private DragMode dragMode = DragMode.CustomEvent;
    [SerializeField] private Axis dragAxis = Axis.Y;

    [Tooltip("Multiplier for mouse delta movement.")]
    [SerializeField] private float sensitivity = 1f;

    [Tooltip("Invert direction of drag calculation.")]
    [SerializeField] private bool invert;

    [Header("Transform Manipulation (for Rotate/Translate mode)")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool useLimits;
    [SerializeField] private float minValue = -180f;
    [SerializeField] private float maxValue = 180f;
    [SerializeField] private float currentValue;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onValueChanged;
    [SerializeField] private UnityEvent<Vector2> onDragDelta;
    [SerializeField] private UnityEvent onDragStarted;
    [SerializeField] private UnityEvent onDragEnded;

    public float Value => currentValue;

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;
    }

    public void OnDragStart(Vector2 screenPosition)
    {
        onDragStarted?.Invoke();
    }

    public void OnDrag(Vector2 screenPosition, Vector2 delta)
    {
        float sign = invert ? -1f : 1f;
        float inputDelta = (dragAxis == Axis.X ? delta.x : delta.y) * sign * sensitivity;

        onDragDelta?.Invoke(delta);

        switch (dragMode)
        {
            case DragMode.CustomEvent:
                currentValue += inputDelta;
                if (useLimits)
                    currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
                onValueChanged?.Invoke(currentValue);
                break;

            case DragMode.RotateTransform:
                ApplyRotation(inputDelta);
                break;

            case DragMode.TranslateTransform:
                ApplyTranslation(inputDelta);
                break;
        }
    }

    public void OnDragEnd(Vector2 screenPosition)
    {
        onDragEnded?.Invoke();
    }

    private void ApplyRotation(float delta)
    {
        if (targetTransform == null) return;

        currentValue += delta;
        if (useLimits)
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        Vector3 axisVector = dragAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.up
        };

        targetTransform.localRotation = Quaternion.AngleAxis(currentValue, axisVector);
        onValueChanged?.Invoke(currentValue);
    }

    private void ApplyTranslation(float delta)
    {
        if (targetTransform == null) return;

        currentValue += delta;
        if (useLimits)
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        Vector3 pos = targetTransform.localPosition;
        switch (dragAxis)
        {
            case Axis.X: pos.x = currentValue; break;
            case Axis.Y: pos.y = currentValue; break;
            case Axis.Z: pos.z = currentValue; break;
        }
        targetTransform.localPosition = pos;
        onValueChanged?.Invoke(currentValue);
    }
}
