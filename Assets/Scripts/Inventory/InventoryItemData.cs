using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Inventory/InventoryItemData")]  
public class InventoryItemData : ScriptableObject
{
    public int id;
    public int itemType; 
    public string itemName;
    public string itemDescription;
    public float itemWeight;
    public float itemPrice;
    public Sprite icon;
    public GameObject prefab;
}
