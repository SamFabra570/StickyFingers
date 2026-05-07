using UnityEngine;

public enum BottleEffect { SecretDoor }

[CreateAssetMenu(fileName = "BottleItemData", menuName = "Inventory/BottleItemData")]
public class BottleItemData : InventoryItemData
{
    [Header("Bottle Settings")]
    public BottleEffect bottleEffect;
}
