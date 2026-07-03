using UnityEngine;
using UnityEngine.UI;

public class InventorySortButton : MonoBehaviour
{
    public InventorySystem.SortMode mode;
    private InventorySystem inventory;

    private void Start()
    {
        inventory = GameObject.Find("InventoryContainer")
            .GetComponent<InventoryContainer>()
            .inventorySystem;
    }

    public void ToggleSorting()
    {
        if (inventory.isSorting)
        {
            if (inventory.sortMode == mode)
            {
                inventory.SortInventory(InventorySystem.SortMode.None);
            }
            else
            {
                inventory.SortInventory(mode);
            }
        }
        else
        {
            inventory.SortInventory(mode);
        }
    }
    
    
}
