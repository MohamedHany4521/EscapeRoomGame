using System;
using FPC_CPP.Runtime;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the UI overlay for inspecting/reading documents in the escape room.
/// Pauses player movement and interaction, hides crosshair, unlocks cursor while reading,
/// and restores the previous player state when closed via Escape.
/// </summary>
public class DocumentInspectionUI : MonoBehaviour
{
    public static DocumentInspectionUI Instance { get; private set; }

    [Header("UI Element References")]
    [Tooltip("Root panel GameObject of the document reading interface.")]
    [SerializeField] private GameObject panelRoot;

    [Tooltip("TextMeshPro component displaying document title.")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Tooltip("TextMeshPro component displaying document body/content.")]
    [SerializeField] private TextMeshProUGUI bodyText;

    [Tooltip("TextMeshPro component displaying the close prompt (e.g. '[ESC] Close').")]
    [SerializeField] private TextMeshProUGUI closePromptText;

    [Header("Player & System References")]
    [Tooltip("Reference to the player controller component to pause/resume movement and mouse look.")]
    [SerializeField] private PlayerController_ControladorDelJugador playerController;

    [Tooltip("Reference to the PlayerInteraction component to pause/resume raycast interaction.")]
    [SerializeField] private PlayerInteraction playerInteraction;

    [Tooltip("Reference to the Crosshair Canvas to hide/show during inspection.")]
    [SerializeField] private Canvas crosshairCanvas;

    private bool isOpen = false;
    private bool savedPlayerControllerEnabled = true;
    private bool savedPlayerInteractionEnabled = true;
    private bool savedCrosshairCanvasEnabled = true;
    private CursorLockMode savedCursorLockMode = CursorLockMode.Locked;
    private bool savedCursorVisible = false;

    public bool IsOpen => isOpen;
    public GameObject PanelRoot => panelRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        ResolveReferences();

        // Ensure reading panel starts inactive safely without deactivating own GameObject
        if (panelRoot != null && panelRoot != gameObject && !isOpen)
        {
            panelRoot.SetActive(false);
        }

        isOpen = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Resolves any unassigned references to player systems or UI elements.
    /// </summary>
    public void ResolveReferences()
    {
        if (panelRoot == null)
        {
            Transform panelTransform = transform.Find("DocumentReadingPanel");
            if (panelTransform != null)
            {
                panelRoot = panelTransform.gameObject;
            }
            else
            {
                GameObject panelObj = GameObject.Find("DocumentReadingPanel");
                if (panelObj != null)
                {
                    panelRoot = panelObj;
                }
            }
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController_ControladorDelJugador>();
        }

        if (playerInteraction == null)
        {
            playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        }

        if (crosshairCanvas == null)
        {
            GameObject crosshairObj = GameObject.Find("CrosshairCanvas");
            if (crosshairObj != null)
            {
                crosshairCanvas = crosshairObj.GetComponent<Canvas>();
            }
        }
    }

    private void Update()
    {
        // Close document only when panel is open and Escape is pressed
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDocument();
        }
    }

    /// <summary>
    /// Opens the document inspection panel, populating title and content,
    /// pausing player controller and interaction, and unlocking cursor.
    /// </summary>
    public void OpenDocument(string title, string content)
    {
        // Guard against duplicate openings
        if (isOpen)
        {
            return;
        }

        ResolveReferences();

        // 1. Save previous state
        savedPlayerControllerEnabled = playerController != null && playerController.enabled;
        savedPlayerInteractionEnabled = playerInteraction != null && playerInteraction.enabled;
        savedCrosshairCanvasEnabled = crosshairCanvas != null && crosshairCanvas.enabled;
        savedCursorLockMode = Cursor.lockState;
        savedCursorVisible = Cursor.visible;

        // 2. Safely stop player movement and physics
        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = Vector3.zero;
#else
                rb.velocity = Vector3.zero;
#endif
                rb.angularVelocity = Vector3.zero;
            }
        }

        // 3. Disable player interaction raycasts
        if (playerInteraction != null)
        {
            playerInteraction.enabled = false;
        }

        // 4. Hide crosshair
        if (crosshairCanvas != null)
        {
            crosshairCanvas.enabled = false;
        }

        // 5. Unlock and display mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6. Populate text contents
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = content;
        }

        if (closePromptText != null && string.IsNullOrEmpty(closePromptText.text))
        {
            closePromptText.text = "[ESC] Close";
        }

        isOpen = true;

        // 7. Show panel
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    /// <summary>
    /// Closes the document inspection panel and restores the saved player, interaction, and cursor states.
    /// </summary>
    public void CloseDocument()
    {
        if (!isOpen)
        {
            return;
        }

        // 1. Hide panel
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        isOpen = false;

        // 2. Restore saved component states
        if (playerController != null)
        {
            playerController.enabled = savedPlayerControllerEnabled;
        }

        if (playerInteraction != null)
        {
            playerInteraction.enabled = savedPlayerInteractionEnabled;
        }

        if (crosshairCanvas != null)
        {
            crosshairCanvas.enabled = savedCrosshairCanvasEnabled;
        }

        // 3. Restore cursor state safely
        if (savedPlayerControllerEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = savedCursorLockMode;
            Cursor.visible = savedCursorVisible;
        }
    }

    private void OnDisable()
    {
        if (isOpen)
        {
            CloseDocument();
        }
    }
}
