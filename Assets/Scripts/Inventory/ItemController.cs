using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private InventoryItemData referenceItem;

    public void Pickup()
    {
        GameManager.Instance.inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
