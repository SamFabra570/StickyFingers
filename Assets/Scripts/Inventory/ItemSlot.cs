using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem item;
    public bool isFull;
    public Sprite emptySprite;
    public GameObject content;
    //public TMP_Text weightText;
    //public TMP_Text valueText;
    public TMP_Text stackValueText;
    public TMP_Text stackWeightText;
    public TMP_Text quantityText;
    
    public Image image;
    
    //public GameObject selectedShader;
    public bool isSelected;

    public void AddItem(InventoryItem source)
    {
        item = source;
        isFull = true;
        
        // weightText.SetText("" + source.data.itemWeight);
        // weightText.enabled = true;
        // valueText.SetText("" + source.data.itemPrice);
        // valueText.enabled = true;
        quantityText.SetText(""+source.stackSize);
        quantityText.enabled = true;
        stackWeightText.SetText(""+source.data.itemWeight);
        stackWeightText.enabled = true;
        stackValueText.SetText(""+source.data.itemPrice);
        stackValueText.enabled = true;

        image.sprite = source.data.icon;
        
        content.SetActive(true);
    }
    public void updateItem()
    {
        if (item.data != null)
        {
            // weightText.SetText("" + item.data.itemWeight);
            // valueText.SetText("" + item.data.itemPrice);
            quantityText.SetText(""+item.stackSize);
            stackWeightText.SetText(""+(item.data.itemWeight * item.stackSize));
            stackValueText.SetText(""+(item.data.itemPrice * item.stackSize));
        }
        else
        {
            image.sprite = emptySprite;
            quantityText.SetText("");
            stackWeightText.SetText("");
            stackValueText.SetText("");
            content.SetActive(false);
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ShowItemDetails();

        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DropItem();
        }
    }

    public void ShowItemDetails()
    {
        InventorySystem inventory = GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
        isSelected = true;
        
        if (item.data != null)
        {
            inventory.content.SetActive(true);
            
            inventory.itemDescriptionNameText.SetText(item.data.itemName);
            inventory.itemDescriptionText.SetText(item.data.itemDescription);
            inventory.itemWeightText.SetText("" + item.data.itemWeight);
            inventory.itemValueText.SetText("" + item.data.itemPrice);

            if (item.data.missionItem)
            {
                if (!inventory.missionItemIcon.activeSelf)
                    inventory.missionItemIcon.SetActive(true);
            }

            else
            {
                if (inventory.missionItemIcon.activeSelf)
                    inventory.missionItemIcon.SetActive(false);
            }
            //inventory.itemDescriptionImage.sprite = item.data.icon;
        }
        else
        {
            inventory.content.SetActive(false);
            
            inventory.itemDescriptionNameText.SetText("");
            inventory.itemDescriptionText.SetText("");
            inventory.itemWeightText.SetText("");
            inventory.itemValueText.SetText("");
            //inventory.missionItemIcon.SetActive(false);
            //inventory.itemDescriptionImage.sprite = emptySprite;
        }

    }
    
    public void DropItem()
    {
        InventorySystem inventory =
            GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;

        bool hadItem = item.data != null;
        inventory.Remove(item.data, PopupUI.PopupType.Dropped);

        // Drop SFX — only when an actual item left the slot
        if (hadItem && PlayerController.Instance != null)
            PlayerController.Instance.GetComponent<PlayerSoundController>()?.PlayDrop();

        inventory.DeselectAllSlots();
        
        UIManager.Instance.UpdateInventoryUI();
    }
}
