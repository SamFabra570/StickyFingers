using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] public InventoryItemData referenceItem;

    public void Pickup()
    {
        GameManager.Instance.inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
