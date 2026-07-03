using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour, IUIMenu
{
    public static InventoryMenu Instance;
    
    [Header ("UI Refs")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject inventoryScreenUI;
    
    [SerializeField] private GameObject firstItem;
    private GameObject lastSelected;

    public Button valueSortButton;
    public Button weightSortButton;
    
    [Header ("Inventory")]
    public InventorySystem inventory;
    private ItemSlot currentItem;
    public GameObject selectionImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

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
        {
            eventSystem.SetSelectedGameObject(firstItem);

            lastSelected = firstItem;
            
            //Debug.Log("Selected item: " + eventSystem.currentSelectedGameObject);
        }
        else
            Debug.Log("First item null, cant select");

        if (!selectionImage.activeSelf)
            selectionImage.SetActive(true);
        
        currentItem = lastSelected.GetComponent<ItemSlot>();
        currentItem.ShowItemDetails();
    }

    public void OnHideMenu()
    {
        lastSelected = null;
        currentItem = null;
        
        inventory.DeselectAllSlots();
        inventory.itemDescriptionNameText.SetText("");
        inventory.itemDescriptionText.SetText("");
        
        UIManager.Instance.HUDCanvas.SetActive(true);
        
        inventoryScreenUI.SetActive(false);
        Debug.Log("Close inventory (InventoryMenu), isActive: " + inventoryScreenUI.activeSelf);

        PlayerController.Instance.isInvOpen = false;
    }
    
    private void Update()
    {
        if (ReferenceEquals(UIMenuStack.Current, this))
        {
            //Debug.Log("Receiving update");
            if (eventSystem.currentSelectedGameObject != lastSelected 
                && eventSystem.currentSelectedGameObject != null)
            {
                //Debug.Log("selected object");
                
                lastSelected = eventSystem.currentSelectedGameObject;

                if (lastSelected.CompareTag("ItemSlot"))
                {
                    currentItem = lastSelected.GetComponent<ItemSlot>();

                    if (!selectionImage.activeSelf)
                        selectionImage.SetActive(true);
                
                    selectionImage.transform.position = currentItem.transform.position;
                    
                    currentItem.ShowItemDetails();
                }
                else if (lastSelected.CompareTag("SortButton"))
                {
                    currentItem = null;
                    selectionImage.SetActive(false);
                }
            }
        }
    }

    public void OnButtonNorth()
    {
        if (currentItem != null) 
            currentItem.DropItem();
        else
        {
            Debug.Log("No item selected");
        }
    }

    public void OnCancel()
    {
        //Debug.Log("Trying to hide inventory");
        UIManager.Instance.HideMenu();
    }
    
}
