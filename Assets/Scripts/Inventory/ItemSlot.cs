using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour
{
    public InventoryItem item;
    public bool isFull = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text priceText;
    public TMP_Text weightText;
    public TMP_Text quantityText;
    
    public Image image;

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
        
        quantityText.SetText(""+this.item.stackSize);
        weightText.SetText(""+(this.item.data.itemWeight*this.item.stackSize));
        priceText.SetText(""+(this.item.data.itemPrice*this.item.stackSize));
    }
}
