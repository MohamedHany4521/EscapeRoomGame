using System;
using UnityEngine;

/// <summary>
/// Serializable data container representing an item in the player's inventory.
/// </summary>
[Serializable]
public class InventoryItem
{
    [SerializeField] private string itemID;
    [SerializeField] private string displayName;
    [SerializeField] private string itemTag;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public string ItemTag => itemTag;
    public string Description => description;

    public InventoryItem()
    {
    }

    public InventoryItem(string itemID, string displayName, string itemTag, string description = "")
    {
        this.itemID = itemID;
        this.displayName = displayName;
        this.itemTag = itemTag;
        this.description = description;
    }
}
