using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public InventoryItemData referenceItem;
    public void OnHandlePickupItem(InventoryItemData source)
    {
        GameManager.Instance.inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHandlePickupItem(referenceItem);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
