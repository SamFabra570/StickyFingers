using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare
}

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Inventory/InventoryItemData")]  
public class InventoryItemData : ScriptableObject
{
    public int id;
    public int itemType; 
    public string itemName;
    
    public bool missionItem;
    public bool isDroppable = true;

    public bool isSafetySlot;
    
    [TextArea]
    public string itemDescription;
    
    public float itemWeight;
    public float itemPrice;

    [Range(0f, 1f)]
    public float spawnRate;
    
    public Sprite icon;
    public GameObject prefab;

    public ItemRarity itemRarity;
}
