using UnityEngine;

[CreateAssetMenu(fileName = "KeyItemData", menuName = "Inventory/KeyItemData")]
public class KeyItemData : InventoryItemData
{
    [Header("Key Settings")]
    public int keyId;
}
