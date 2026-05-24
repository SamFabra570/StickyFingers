using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem item;
    public bool isFull;
    public Sprite emptySprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text priceText;
    public TMP_Text weightText;
    public TMP_Text quantityText;
    
    public Image image;
    
    public GameObject selectedShader;
    public bool isSelected;

    public void AddItem(InventoryItem source)
    {
        item = source;
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
        if (item.data != null)
        {
            quantityText.SetText(""+item.stackSize);
            weightText.SetText(""+(item.data.itemWeight * item.stackSize));
            priceText.SetText(""+(item.data.itemPrice * item.stackSize));
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
            ShowItemDetails();

        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DropItem();
        }
    }

    public void ShowItemDetails()
    {
        InventorySystem inventory =
            GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
        inventory.DeselectAllSlots();
        selectedShader.SetActive(true);
        isSelected = true;
        if (item.data != null)
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
    
    public void DropItem()
    {
        InventorySystem inventory =
            GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;

        bool hadItem = item.data != null;
        isFull = false;
        inventory.Remove(item.data);

        // Drop SFX — only when an actual item left the slot
        if (hadItem && PlayerController.Instance != null)
            PlayerController.Instance.GetComponent<PlayerSoundController>()?.PlayDrop();

        inventory.DeselectAllSlots();
    }
}
