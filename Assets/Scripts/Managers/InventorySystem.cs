using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class InventorySystem 
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory;
    public ItemSlot[] itemSlots;
    public int freeSlot = 0;
    public float totalWeight;
    public float totalBounty;

    public InventorySystem()
    {
        inventory= new List<InventoryItem>();
        m_itemDictionary= new Dictionary<InventoryItemData, InventoryItem>();
        totalBounty = 0;
        totalWeight = 0;
    }

    public void Add(InventoryItemData referenceData)
    {
        totalBounty += referenceData.itemPrice;
        totalWeight += referenceData.itemWeight;
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
            refreshInventory();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
            itemSlots[freeSlot].AddItem(newItem);
            freeSlot++;
        }
        UIManager.Instance.UpdateTotals();
        UIManager.Instance.ShowItemPickupNotif(referenceData);
    }
    
    public void Remove(InventoryItemData referenceData)
    {
        
        
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                totalBounty -= referenceData.itemPrice;
                totalWeight -= referenceData.itemWeight;
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }

        UIManager.Instance.ShowItemRemoveNotif(referenceData);
        UIManager.Instance.UpdateTotals();
    }

    public void refreshInventory()
    {
        for(int i=0; i<freeSlot; i++)
        {
            itemSlots[i].updateItem();
        }
    }

    private void Update()
    {
        //Debug.Log(inventory.Count);
    }
}
