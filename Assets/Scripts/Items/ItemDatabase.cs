using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [Header("Item Prefabs")]
    public GameObject bronzePrefab;
    public GameObject silverPrefab;
    public GameObject goldPrefab;
    public GameObject missionPrefab;
    
    [Header("Items")]
    public List<InventoryItemData> bronzeItems = new();
    public List<InventoryItemData> silverItems = new();
    public List<InventoryItemData> goldItems = new();
    public List<InventoryItemData> missionItems = new();
    
    public GameObject GetPrefab(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Bronze => bronzePrefab,
            ItemRarity.Silver => silverPrefab,
            ItemRarity.Gold => goldPrefab,
            ItemRarity.Mission => missionPrefab,
            _ => null
        };
    }
    
    public InventoryItemData GetRandomItemByRarity(ItemRarity rarity)
    {
        List<InventoryItemData> items = GetItemsByRarity(rarity);

        if (items == null || items.Count == 0)
            return null;

        return items[Random.Range(0, items.Count)];
    }

    private List<InventoryItemData> GetItemsByRarity(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Bronze => bronzeItems,
            ItemRarity.Silver => silverItems,
            ItemRarity.Gold => goldItems,
            ItemRarity.Mission => missionItems,
            _ => new List<InventoryItemData>()
        };
    }
}
