using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public static PopupUI Instance;
    
    [Header ("Object Ref")]
    public GameObject objectPickupUI;

    [Header("Content")] 
    public Image icon;
    public TextMeshProUGUI textNameNotif;
    public TextMeshProUGUI textRemoveNameNotif;
    public TextMeshProUGUI textWeightNotif;
    public TextMeshProUGUI textValueNotif;
    
    public enum PopupType
    {
        Pickup,
        Dropped,
        Stolen
    }
    
    public PopupType popupType;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPopupContent(InventoryItemData itemData, PopupType state)
    {
        switch (state)
        {
            case PopupType.Pickup:
                textNameNotif.color = Color.black;
                textWeightNotif.color = Color.black;
                textValueNotif.color = Color.black;
                break;
            case PopupType.Dropped:
                textNameNotif.color = Color.yellowNice;
                textWeightNotif.color = Color.yellowNice;
                textValueNotif.color = Color.yellowNice;
                break;
            case PopupType.Stolen:
                textNameNotif.color = Color.darkRed;
                textWeightNotif.color = Color.darkRed;
                textValueNotif.color = Color.darkRed;
                break;
        }
        
        textNameNotif.SetText(itemData.itemName);
        textWeightNotif.SetText("" + itemData.itemWeight);
        textValueNotif.SetText("" + itemData.itemPrice);
    }
}
