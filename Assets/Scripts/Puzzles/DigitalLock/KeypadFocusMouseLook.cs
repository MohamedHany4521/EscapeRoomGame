
using UnityEngine;

public class KeypadFocusMouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 45f;
    [SerializeField] private float yawLimit = 45f;

    private float yaw;
    private float pitch;
    private float initialYaw;

    private void OnEnable()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        initialYaw = yaw;

        pitch = angles.x;
        if (pitch > 180f)
            pitch -= 360f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        float minYaw = initialYaw - yawLimit;
        float maxYaw = initialYaw + yawLimit;
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
