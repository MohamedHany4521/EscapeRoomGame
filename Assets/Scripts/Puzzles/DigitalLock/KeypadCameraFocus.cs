
using UnityEngine;

public class KeypadCameraFocus : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Behaviour playerController;
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("Cameras")]
    [SerializeField] private Camera normalCamera;
    [SerializeField] private Camera keypadCamera;

    [Header("Focus Controls")]
    [SerializeField] private KeyCode exitFocusKey = KeyCode.Escape;
    [SerializeField] private KeypadFocusMouseLook mouseLook;

    private bool isFocused;
    private bool previousControllerState;
    private RigidbodyConstraints originalConstraints;

    private void Awake()
    {
        if (keypadCamera != null && mouseLook == null)
            mouseLook = keypadCamera.GetComponent<KeypadFocusMouseLook>();

        if (keypadCamera != null)
            keypadCamera.enabled = false;

        if (mouseLook != null)
            mouseLook.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFocused || player == null)
            return;

        if (other.transform == player ||
            other.transform.IsChildOf(player))
        {
            FocusOnKeypad();
        }
    }

    private void Update()
    {
        if (isFocused && Input.GetKeyDown(exitFocusKey))
            ExitFocus();
    }

    private void FocusOnKeypad()
    {
        if (normalCamera == null || keypadCamera == null)
        {
            Debug.LogError("Assign both cameras in KeypadCameraFocus.", this);
            return;
        }

        isFocused = true;

        if (playerController != null)
        {
            previousControllerState = playerController.enabled;
            playerController.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;

            originalConstraints = playerRigidbody.constraints;
            playerRigidbody.constraints |=
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ;
        }

        normalCamera.enabled = false;
        keypadCamera.enabled = true;

        if (mouseLook != null)
            mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("KEYPAD FOCUS COMPLETE - Press ESC to exit");
    }

    private void ExitFocus()
    {
        isFocused = false;

        if (mouseLook != null)
            mouseLook.enabled = false;

        if (keypadCamera != null)
            keypadCamera.enabled = false;

        if (normalCamera != null)
            normalCamera.enabled = true;

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        if (playerController != null)
            playerController.enabled = previousControllerState;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("KEYPAD FOCUS EXITED");
    }
}
