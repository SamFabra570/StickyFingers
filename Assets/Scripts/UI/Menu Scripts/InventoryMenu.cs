using System;
using System.Collections.Generic;
using UnityEditor.MPE;
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
    public GameObject selectionImage;
    
    public GameObject safetySlotImage;
    public GameObject safetySlotButton;
    
    public ItemSlot safetySlot;
    
    public Transform inventoryDescriptionBackground;
    
    [HideInInspector] public InventorySystem inventory;
    private ItemSlot currentItemSlot;
    
    
    [Header("Tutorial")] 
    public List<TutorialSegment> inventoryTutorial = new();
    public List<Transform> inventoryTutorialElements = new();

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

        if (!TutorialMenu.Instance.HasCompletedTutorial(inventoryTutorial[0]))
        {
            selectionImage.SetActive(false);
            
            TutorialMenu.Instance.CacheTutorialContent(inventoryTutorial, inventoryTutorialElements);
            
            UIManager.Instance.OpenMenu("TutorialMenu");
            return;
        }
        
        inventoryDescriptionBackground.SetAsFirstSibling();

        if (firstItem != null)
        {
            eventSystem.SetSelectedGameObject(firstItem);

            lastSelected = firstItem;
            
            //Debug.Log("Selected item: " + eventSystem.currentSelectedGameObject);
        }
        else
            Debug.Log("First item null, cant select");
        
        if (!GameManager.Instance.PlayerPassives.Has(PassiveAbilities.SafetySlot)) 
            safetySlotButton.SetActive(false);

        if (!selectionImage.activeSelf)
            selectionImage.SetActive(true);

        if (safetySlot != null) 
            safetySlotImage.SetActive(true);
        else 
            safetySlotImage.SetActive(false);
        
        selectionImage.transform.SetAsFirstSibling();
        
        currentItemSlot = lastSelected.GetComponent<ItemSlot>();
        currentItemSlot.ShowItemDetails();
        selectionImage.transform.position = currentItemSlot.transform.position;
    }

    public void OnHideMenu()
    {
        lastSelected = null;
        currentItemSlot = null;
        
        inventory.DeselectSlot();
        inventory.itemDescriptionNameText.SetText("");
        inventory.itemDescriptionText.SetText("");
        
        UIManager.Instance.HUDCanvas.SetActive(true);
        
        inventoryScreenUI.SetActive(false);
        //Debug.Log("Close inventory (InventoryMenu), isActive: " + inventoryScreenUI.activeSelf);

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
                    currentItemSlot = lastSelected.GetComponent<ItemSlot>();

                    if (!selectionImage.activeSelf)
                        selectionImage.SetActive(true);
                    
                    if (GameManager.Instance.PlayerPassives.Has(PassiveAbilities.SafetySlot)) 
                        safetySlotButton.SetActive(true);
                
                    selectionImage.transform.position = currentItemSlot.transform.position;
                    
                    //Debug.Log("sSHOW BITACHASSS");
                    
                    currentItemSlot.ShowItemDetails();
                }
                else if (lastSelected.CompareTag("SortButton"))
                {
                    currentItemSlot = null;
                    selectionImage.SetActive(false);
                    safetySlotButton.SetActive(false);
                    inventory.dropItemText.SetActive(false);
                }
            }
        }
    }

    private void ToggleSafetySlotItem()
    {
        if (safetySlot != null)
        {
            if (safetySlot.item.data == currentItemSlot.item.data)
            {
                safetySlot.item.data.isSafetySlot = false;
                safetySlot = null;
                
                safetySlotImage.SetActive(false);

                return;
            }
            
            safetySlot.item.data.isSafetySlot = false;
        }
        
        safetySlotImage.transform.position = currentItemSlot.transform.position;
        safetySlotImage.SetActive(true);
        
        safetySlot = currentItemSlot;
        safetySlot.item.data.isSafetySlot = true;
    }

    public void OnButtonNorth()
    {
        if (currentItemSlot.item.data != null) 
            currentItemSlot.DropItem();
        else
        {
            Debug.Log("No item selected");
        }
    }

    public void OnButtonWest()
    {
        if (currentItemSlot.item.data != null)
        {
            if (!GameManager.Instance.PlayerPassives.Has(PassiveAbilities.SafetySlot)) return;
        
            ToggleSafetySlotItem();
        }
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
