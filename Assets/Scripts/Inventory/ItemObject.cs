using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(InventoryItemData source)
    {
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
