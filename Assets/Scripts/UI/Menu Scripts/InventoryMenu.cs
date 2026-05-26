using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject inventoryScreenUI;
    
    [SerializeField] private GameObject firstItem;
    private GameObject lastSelected;
    
    private ItemSlot currentItem;
    
    public void OnShowMenu()
    {
        GameManager.Instance.PauseGame(1);
        PlayerController.Instance.isPaused = true;
        
        inventoryScreenUI.SetActive(true);
        
        if (firstItem != null)
            eventSystem.SetSelectedGameObject(firstItem);
    }

    public void OnHideMenu()
    {
        lastSelected = null;
        currentItem = null;
        
        GameManager.Instance.PauseGame(0);
        PlayerController.Instance.isPaused = false;
        
        inventoryScreenUI.SetActive(false);
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
        UIMenuStack.Pop();
        UIManager.Instance.HideMenu("Inventory");
    }
    
    public void OnSubmit() { }
    
}
