using UnityEngine;

public interface IPuzzleDraggable
{
    void OnDragStart(Vector2 screenPosition);
    void OnDrag(Vector2 screenPosition, Vector2 delta);
    void OnDragEnd(Vector2 screenPosition);
}
