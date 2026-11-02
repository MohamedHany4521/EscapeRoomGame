using UnityEngine;
using UnityEngine.Events;

public class PuzzleClickable : MonoBehaviour, IPuzzleClickable
{
    [Header("Click Action")]
    [SerializeField] private UnityEvent onClick;

    [Header("Visual Feedback")]
    [SerializeField] private Outline outline;

    public bool IsClickable => enabled && gameObject.activeInHierarchy;

    private void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public void OnPuzzleClick()
    {
        if (!IsClickable)
            return;

        onClick?.Invoke();
    }
}
