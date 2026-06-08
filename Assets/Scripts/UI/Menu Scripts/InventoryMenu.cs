using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour, IUIMenu
{
    [Header ("UI Refs")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject inventoryScreenUI;
    
    [SerializeField] private GameObject firstItem;
    private GameObject lastSelected;
    
    [Header ("Inventory")]
    public InventorySystem inventory;
    public Sprite emptySprite;
    private ItemSlot currentItem;

    private void Start()
    {
        inventory = UIManager.Instance.inventory;
    }

    public void OnShowMenu()
    {
        PlayerController.Instance.isInvOpen = true;
        
        UIManager.Instance.HUDCanvas.SetActive(false);
        inventoryScreenUI.SetActive(true);
        
        if (firstItem != null)
            eventSystem.SetSelectedGameObject(firstItem);
    }

    public void OnHideMenu()
    {
        lastSelected = null;
        currentItem = null;
        
        inventory.DeselectAllSlots();
        inventory.itemDescriptionNameText.SetText("");
        inventory.itemDescriptionText.SetText("");
        inventory.itemDescriptionImage.sprite = emptySprite;
        
        UIManager.Instance.HUDCanvas.SetActive(true);
        inventoryScreenUI.SetActive(false);

        PlayerController.Instance.isInvOpen = false;
    }
    
    private void Update()
    {
        if (ReferenceEquals(UIMenuStack.Current, this))
        {
            if (eventSystem.currentSelectedGameObject != lastSelected)
            {
                lastSelected = eventSystem.currentSelectedGameObject;
                currentItem = lastSelected.GetComponent<ItemSlot>();

                currentItem.ShowItemDetails();
            }
        }
    }

    public void OnButtonNorth()
    {
        currentItem.DropItem();
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
    
}
