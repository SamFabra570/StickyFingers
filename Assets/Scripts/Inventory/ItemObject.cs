using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(InventoryItemData source)
    {
        GameManager.Instance.inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
