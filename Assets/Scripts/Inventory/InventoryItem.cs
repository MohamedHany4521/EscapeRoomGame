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
    [SerializeField] private Sprite icon;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public string ItemTag => itemTag;
    public string Description => description;
    public Sprite Icon => icon;

    public InventoryItem()
    {
    }

    public InventoryItem(string itemID, string displayName, string itemTag, string description = "", Sprite icon = null)
    {
        this.itemID = itemID;
        this.displayName = displayName;
        this.itemTag = itemTag;
        this.description = description;
        this.icon = icon;
    }
}
