using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    [Header ("Object Ref")]
    public GameObject objectPickupUI;

    [Header("Content")] 
    public Image icon;
    public TextMeshProUGUI textNameNotif;
    public TextMeshProUGUI textWeightNotif;
    public GameObject weightIcon;
    public TextMeshProUGUI textValueNotif;
    public GameObject valueIcon;
    
    public TextMeshProUGUI notificationText;
    
    public enum PopupType
    {
        Pickup,
        Dropped,
        Stolen,
        SafetySlot,
        Consumed,
        NeedKey
    }
    
    public PopupType popupType;

    public void SetPopupContent(InventoryItemData itemData, PopupType state)
    {
        ShowItemInfo();
        
        switch (state)
        {
            case PopupType.Pickup:

                switch (itemData.itemRarity)
                {
                    case ItemRarity.Bronze:
                        SetTextColour(Color.sienna);
                        break;
                    case ItemRarity.Silver:
                        SetTextColour(Color.lightSlateGray);
                        break;
                    case ItemRarity.Gold:
                        SetTextColour(Color.goldenRod);
                        break;
                }
                
                SetIconImage(itemData);
                
                textNameNotif.SetText(itemData.itemName);
                textWeightNotif.SetText("" + itemData.itemWeight);
                textValueNotif.SetText("" + itemData.itemPrice);
                break;
            case PopupType.Dropped:
                SetTextColour(Color.black);
                SetIconImage(itemData);
                
                textNameNotif.SetText("- " + itemData.itemName);
                textWeightNotif.SetText("-" + itemData.itemWeight);
                textValueNotif.SetText("-" + itemData.itemPrice);
                break;
            case PopupType.Stolen:
                SetTextColour(Color.darkRed);

                //If no items left
                if (itemData == null)
                {
                    HideItemInfo();
                    
                    notificationText.SetText("No items left in inventory!");
                    return;
                }
                
                SetIconImage(itemData);
                
                textNameNotif.SetText(itemData.itemName + " stolen!");
                textWeightNotif.SetText("-" + itemData.itemWeight);
                textValueNotif.SetText("-" + itemData.itemPrice);
                break;
            case PopupType.SafetySlot:
                SetTextColour(Color.purple);
                
                SetIconImage(itemData);
                
                HideItemInfo();
                notificationText.SetText(itemData.itemName + " protected by safety slot!");
                break;
            case PopupType.Consumed:
                SetTextColour(Color.black);
                SetIconImage(itemData);
                
                if (itemData.name == "Key")
                {
                    HideItemInfo();
                    notificationText.SetText("Used key!");
                }
                break;
            case PopupType.NeedKey:
                SetTextColour(Color.darkRed);
                
                HideItemInfo();
                notificationText.SetText("No keys in inventory!");
                break;
        }
    }

    private void ShowItemInfo()
    {
        notificationText.gameObject.SetActive(false);
        
        weightIcon.SetActive(true);
        valueIcon.SetActive(true);
        icon.gameObject.SetActive(true);
    }
    
    private void HideItemInfo()
    {
        textNameNotif.SetText("");
        weightIcon.SetActive(false);
        valueIcon.SetActive(false);
        icon.gameObject.SetActive(false);
        
        notificationText.gameObject.SetActive(true);
    }

    private void SetTextColour(Color colour)
    {
        notificationText.color = colour;
        textNameNotif.color = colour;
        textWeightNotif.color = colour;
        textValueNotif.color = colour;
    }

    private void SetIconImage(InventoryItemData itemData)
    {
        if (itemData.icon != null)
        {
            icon.sprite = itemData.icon;
            icon.gameObject.SetActive(true);
        }
        else
            icon.gameObject.SetActive(false);
    }
}
