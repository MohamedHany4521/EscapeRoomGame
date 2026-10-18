using System;
using System.Buffers.Text;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI.Table;

public class ItemInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance at which the player can interact.")]
    public float interactDistance = 3f;

    [Tooltip("Key to press for interaction.")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Events")]
    public UnityEvent onInteract; // Event triggered when interaction happens

    private Transform player;

    void Start()
    {
        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Please tag your player object as 'Player'.");
        }

        // Ensure collider is set to trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Check distance
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= interactDistance)
        {
            // Show prompt (optional)
            ShowInteractionPrompt();

            // Check key press
            if (UnityEngine.Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }
    }

    /// <summary>
    /// Handles the interaction logic.
    /// </summary>
    public void Interact()
    {
        Debug.Log($"{gameObject.name} interacted with.");
        onInteract?.Invoke(); // Trigger UnityEvent
    }

    /// <summary>
    /// Displays an interaction prompt (optional).
    /// Replace with UI logic.
    /// </summary>
    private void ShowInteractionPrompt()
    {
        // Example: Debug prompt
        Debug.Log($"Press {interactKey} to interact with {gameObject.name}");
    }
    public void PickupItem()
    {
        Debug.Log("Item picked up!");
        Destroy(gameObject);
    }

}
