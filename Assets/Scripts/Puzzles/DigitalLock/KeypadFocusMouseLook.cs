using UnityEngine;

public class KeypadFocusMouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 45f;
    [SerializeField] private float yawLimit = 45f;

    private Quaternion initialLocalRotation;
    private float yaw;
    private float pitch;

    private void OnEnable()
    {
        initialLocalRotation = transform.localRotation;

        yaw = 0f;
        pitch = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        yaw = Mathf.Clamp(
            yaw + mouseX * sensitivity,
            -yawLimit,
            yawLimit
        );

        pitch = Mathf.Clamp(
            pitch - mouseY * sensitivity,
            minPitch,
            maxPitch
        );

        transform.localRotation =
            initialLocalRotation * Quaternion.Euler(pitch, yaw, 0f);
    }
}