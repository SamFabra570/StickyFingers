using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(InventoryItemData source)
    {
        InventorySystem.instance.Add(referenceItem);
        Destroy(gameObject);
    }
}
