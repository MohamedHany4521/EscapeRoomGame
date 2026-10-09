
using UnityEngine;

public class KeypadRaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rayDistance = 5f;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponent<Camera>();

        if (playerCamera == null)
            Debug.LogError("Player Camera is not assigned!", this);
    }

    private void Update()
    {
        if (playerCamera == null || !playerCamera.enabled)
            return;

        if (!Input.GetKeyDown(interactKey))
            return;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            KeypadButtonInteraction button =
                hit.collider.GetComponentInParent<KeypadButtonInteraction>();

            if (button != null)
            {
                button.Press();
                Debug.Log("Keypad button pressed: " + hit.collider.name);
            }
        }
    }
}