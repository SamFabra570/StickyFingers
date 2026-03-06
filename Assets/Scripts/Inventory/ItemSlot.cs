using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem item=null;
    public bool isFull = false;
    public Sprite emptySprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text priceText;
    public TMP_Text weightText;
    public TMP_Text quantityText;
    
    public Image image;
    
    public GameObject selectedShader;
    public bool isSelected = false;

    public void AddItem(InventoryItem source)
    {
        this.item = source;
        isFull = true;
        
        quantityText.SetText(""+source.stackSize);
        quantityText.enabled = true;
        weightText.SetText(""+source.data.itemWeight);
        weightText.enabled = true;
        priceText.SetText(""+source.data.itemPrice);
        priceText.enabled = true;

        image.sprite = source.data.icon;
    }
    public void updateItem()
    {
        if (this.item.data != null)
        {
            quantityText.SetText(""+this.item.stackSize);
            weightText.SetText(""+(this.item.data.itemWeight*this.item.stackSize));
            priceText.SetText(""+(this.item.data.itemPrice*this.item.stackSize));
        }
        else
        {
            image.sprite = emptySprite;
            quantityText.SetText("");
            weightText.SetText("");
            priceText.SetText("");
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();

        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        InventorySystem inventory =
            GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
        inventory.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;
        if (this.item.data != null)
        {
            inventory.itemDescriptionNameText.SetText(item.data.itemName);
            inventory.itemDescriptionText.SetText(item.data.itemDescription);
            inventory.itemDescriptionImage.sprite = item.data.icon;
        }
        else
        {
            inventory.itemDescriptionNameText.SetText("");
            inventory.itemDescriptionText.SetText("");
            inventory.itemDescriptionImage.sprite = emptySprite;
        }

    }
    
    public void OnRightClick()
    {
        InventorySystem inventory =
            GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
        inventory.Remove(item.data);
        inventory.DeselectAllSlots();
    }
}
