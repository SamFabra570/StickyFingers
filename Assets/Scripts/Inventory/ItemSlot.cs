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
        GameManager.Instance.inventorySystem.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;
        if (this.item.data != null)
        {
            GameManager.Instance.inventorySystem.itemDescriptionNameText.SetText(item.data.itemName);
            GameManager.Instance.inventorySystem.itemDescriptionText.SetText(item.data.itemDescription);
            GameManager.Instance.inventorySystem.itemDescriptionImage.sprite = item.data.icon;
        }
        else
        {
            GameManager.Instance.inventorySystem.itemDescriptionNameText.SetText("");
            GameManager.Instance.inventorySystem.itemDescriptionText.SetText("");
            GameManager.Instance.inventorySystem.itemDescriptionImage.sprite = emptySprite;
        }

    }
    
    public void OnRightClick()
    {
        GameManager.Instance.inventorySystem.Remove(item.data);
        GameManager.Instance.inventorySystem.DeselectAllSlots();
    }
}
