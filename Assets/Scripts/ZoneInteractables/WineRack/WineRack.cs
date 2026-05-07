using UnityEngine;
using ZoneInteractables;

public class WineRack : MonoBehaviour, IInteractable
{
    [Header("Secret Door")]
    [SerializeField] private GameObject secretDoor;

    private bool _bottlePlaced = false;

    public void Interact(GameObject player)
    {
        if (_bottlePlaced) return;

        var inventory = GetInventory();
        if (inventory == null) return;

        if (!FindBottleInInventory(inventory, out BottleItemData bottle))
        {
            Debug.Log("[WineRack] No bottle in inventory.");
            return;
        }

        inventory.Remove(bottle);
        _bottlePlaced = true;
        TriggerEffect(bottle.bottleEffect);
    }

    private void TriggerEffect(BottleEffect effect)
    {
        switch (effect)
        {
            case BottleEffect.SecretDoor:
                OpenSecretDoor();
                break;
        }
    }

    private void OpenSecretDoor()
    {
        if (secretDoor != null)
            secretDoor.SetActive(false);
        else
            Debug.LogWarning("[WineRack] No secret door assigned.");
    }

    private bool FindBottleInInventory(InventorySystem inventory, out BottleItemData found)
    {
        found = null;
        foreach (var item in inventory.inventory)
        {
            if (item.data is BottleItemData bottle)
            {
                found = bottle;
                return true;
            }
        }
        return false;
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[WineRack] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
