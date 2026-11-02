using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages items collected by the player.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    /// <summary>
    /// Adds an item to the inventory. Rejects null items, empty item IDs, and duplicates.
    /// </summary>
    public bool AddItem(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("PlayerInventory: Cannot add null item.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(item.ItemID))
        {
            Debug.LogWarning("PlayerInventory: Cannot add item with null or empty ItemID.");
            return false;
        }

        if (HasItem(item.ItemID))
        {
            Debug.LogWarning($"PlayerInventory: Item with ID '{item.ItemID}' already exists in inventory.");
            return false;
        }

        items.Add(item);
        Debug.Log($"PlayerInventory: Added '{item.DisplayName}' ({item.ItemID}). Total items: {items.Count}");
        return true;
    }

    /// <summary>
    /// Checks whether an item with the given ID exists in the inventory.
    /// </summary>
    public bool HasItem(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return false;

        return items.Exists(i => i != null && string.Equals(i.ItemID, itemID, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks whether an item with the given tag exists in the inventory.
    /// </summary>
    public bool HasItemWithTag(string itemTag)
    {
        if (string.IsNullOrWhiteSpace(itemTag))
            return false;

        return items.Exists(i => i != null && string.Equals(i.ItemTag, itemTag, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Removes an item by ID from the inventory. Returns false if the item does not exist.
    /// </summary>
    public bool RemoveItem(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return false;

        int index = items.FindIndex(i => i != null && string.Equals(i.ItemID, itemID, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
            return false;

        InventoryItem removed = items[index];
        items.RemoveAt(index);
        Debug.Log($"PlayerInventory: Removed '{removed.DisplayName}' ({removed.ItemID}). Total items: {items.Count}");
        return true;
    }

    /// <summary>
    /// Retrieves an item by its ID. Returns null if not found.
    /// </summary>
    public InventoryItem GetItem(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return null;

        return items.Find(i => i != null && string.Equals(i.ItemID, itemID, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns a read-only list of all items currently in the inventory.
    /// </summary>
    public IReadOnlyList<InventoryItem> GetItems()
    {
        return items.AsReadOnly();
    }
}
