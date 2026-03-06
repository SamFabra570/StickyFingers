using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] public InventoryItemData referenceItem;

    public void Pickup()
    {
        GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
