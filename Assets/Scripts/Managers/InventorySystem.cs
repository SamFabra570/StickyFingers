using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[Serializable]
public class InventorySystem 
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory;
    public ItemSlot[] itemSlots;
    public int freeSlot;
    public float totalWeight;
    public float totalBounty;

    public GameObject content;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemWeightText;
    public TMP_Text itemValueText;
    public GameObject missionItemIcon;
    public GameObject dropItemText;
    
    private int nextPickupOrder;

    public bool isSorting;
    
    public enum SortMode
    {
        None,
        Value,
        Weight
    }
    public SortMode sortMode = SortMode.None;

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
            newItem.pickupOrder = nextPickupOrder++;
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
            itemSlots[freeSlot].AddItem(newItem);
            freeSlot++;
        }

        UIManager.Instance.UpdateTotals();
        UIManager.Instance.ShowItemPopupUI(referenceData, PopupUI.PopupType.Pickup);
    }
    
    public void Remove(InventoryItemData referenceData, PopupUI.PopupType popupType)
    {
        if (referenceData != null)
        {
            if (referenceData.isSafetySlot)
            {
                UIManager.Instance.ShowItemPopupUI(referenceData, PopupUI.PopupType.SafetySlot);
                return;
            }
            
            if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                value.RemoveFromStack();
                totalBounty -= referenceData.itemPrice;
                totalWeight -= referenceData.itemWeight;
                if (value.stackSize == 0)
                {
                    itemDescriptionNameText.SetText("");
                    itemDescriptionText.SetText("");
                    itemWeightText.SetText("");
                    itemValueText.SetText("");
                    //itemDescriptionImage.sprite = emptySprite;
                    
                    inventory.Remove(value);
                    m_itemDictionary.Remove(referenceData);
                    freeSlot--;
                    
                    content.gameObject.SetActive(false);
                    //DeselectSlot();
                }
                RefreshInventory();
            }
            UIManager.Instance.ShowItemPopupUI(referenceData, popupType);
            UIManager.Instance.UpdateTotals();
        }
        else 
            UIManager.Instance.ShowItemPopupUI(referenceData, popupType);
    }

    public void DeselectSlot()
    {
        if (PlayerController.Instance.isInvOpen) 
            InventoryMenu.Instance.selectionImage.SetActive(false);
        
        // for (int i = 0; i < itemSlots.Length; i++)
        // {
        //     
        //     //Debug.Log("Selection image off (from inventory system)");
        //     //itemSlots[i].selectedShader.SetActive(false);
        // }
    }

    public void SortInventory(SortMode mode)
    {
        sortMode = mode;

        switch (mode)
        {
            case SortMode.None:
                inventory.Sort((a, b) => a.pickupOrder.CompareTo(b.pickupOrder));
                
                InventoryMenu.Instance.valueSortButton.GetComponent<Image>().color = InventoryMenu.Instance.valueSortButton.colors.normalColor;
                InventoryMenu.Instance.weightSortButton.GetComponent<Image>().color = InventoryMenu.Instance.weightSortButton.colors.normalColor;
                isSorting = false;
                break;

            case SortMode.Value:
                inventory.Sort((a, b) => (b.data.itemPrice).CompareTo(a.data.itemPrice));
                
                InventoryMenu.Instance.valueSortButton.GetComponent<Image>().color = Color.gray3;
                InventoryMenu.Instance.weightSortButton.GetComponent<Image>().color = InventoryMenu.Instance.weightSortButton.colors.normalColor;
                isSorting = true;
                break;

            case SortMode.Weight:
                inventory.Sort((a, b) => (b.data.itemWeight).CompareTo(a.data.itemWeight));
                
                InventoryMenu.Instance.weightSortButton.GetComponent<Image>().color = Color.gray3;
                InventoryMenu.Instance.valueSortButton.GetComponent<Image>().color = InventoryMenu.Instance.valueSortButton.colors.normalColor;
                isSorting = true;
                break;
        }
        
        RefreshInventory();
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
    }
    
    
    public void SellInventory(bool runState)
    {
        //check safe slot-pending for dev
        if (runState)
        {
            GameManager.Instance.SavePostGameInventory(inventory);
            
            GameManager.Instance.remainingDebt -= totalBounty;
        }
        else
        {
            if (GameManager.Instance.PlayerPassives.Has(PassiveAbilities.SafetySlot))
            {
                if (InventoryMenu.Instance.safetySlot != null)
                {
                    GameManager.Instance.remainingDebt -= (InventoryMenu.Instance.safetySlot.item.stackSize * InventoryMenu.Instance.safetySlot.item.data.itemPrice);
                }
            }
            else
            {
                Debug.Log("Run failed, lost all ur loot lol");
            }
            
        }

    }

    private void Update()
    {
        //Debug.Log(inventory.Count);
    }
}
