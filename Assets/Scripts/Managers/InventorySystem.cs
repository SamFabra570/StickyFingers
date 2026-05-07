using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
[Serializable]
public class InventorySystem 
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory;
    public ItemSlot[] itemSlots;
    public int freeSlot = 0;
    public float totalWeight;
    public float totalBounty;
    
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    
    public Sprite emptySprite;

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
            RefreshInventory();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
            itemSlots[freeSlot].AddItem(newItem);
            freeSlot++;
        }

        GameManager.Instance.UpdateSpeed(totalWeight);
        UIManager.Instance.UpdateTotals();
        UIManager.Instance.ShowItemPickupNotif(referenceData);
    }
    
    public void Remove(InventoryItemData referenceData)
    {

        if (referenceData != null)
        {
            if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                value.RemoveFromStack();
                totalBounty -= referenceData.itemPrice;
                totalWeight -= referenceData.itemWeight;
                if (value.stackSize == 0)
                {
                    inventory.Remove(value);
                    m_itemDictionary.Remove(referenceData);
                    freeSlot--;
                }
                RefreshInventory();
            }
            GameManager.Instance.UpdateSpeed(totalWeight);
            UIManager.Instance.ShowItemRemoveNotif(referenceData);
            UIManager.Instance.UpdateTotals();
        }

        
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].selectedShader.SetActive(false);
        }
    }

    public void RefreshInventory()
    {
        int i = 0;
        if (inventory.Count == 0)
        {
            itemSlots[0].item.data=null;
            itemSlots[0].item.stackSize=0;
            itemSlots[0].updateItem();
        }

        while (i < inventory.Count)
        {
            itemSlots[i].item = inventory[i];
            itemSlots[i].updateItem();
            i++;
        }

        for(int j=i+1; i<itemSlots.Length; i++)
        {
            itemSlots[i].item = new InventoryItem(null);
            itemSlots[i].item.stackSize = 0;
            itemSlots[i].updateItem();
        }
        DeselectAllSlots();
        /*GameManager.Instance.inventorySystem.itemDescriptionNameText.SetText("");
        GameManager.Instance.inventorySystem.itemDescriptionText.SetText("");
        GameManager.Instance.inventorySystem.itemDescriptionImage.sprite = emptySprite;*/
        itemDescriptionNameText.SetText("");
        itemDescriptionText.SetText("");
        itemDescriptionImage.sprite = emptySprite;
    }
    
    
    public void SellInventory(bool runState)
    {
        //check safe slot-pending for dev
        if (runState)
        {
            GameManager.Instance.remainingDebt-= totalBounty;
        }
        else
        {
        }

    }

    private void Update()
    {
        //Debug.Log(inventory.Count);
    }
}
