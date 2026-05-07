using UnityEngine;
using ZoneInteractables;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private ObjectSpawner objectSpawner;

    [Header("Lock Settings")]
    [SerializeField] private int requiredKeyId;

    private bool _isOpen = false;

    public void Interact(GameObject player)
    {
        if (_isOpen) return;

        if (HasMatchingKey(out KeyItemData key))
        {
            ConsumeKey(key);
            Open();
            return;
        }

        if (HasLockpick())
        {
            Open();
            return;
        }

        Debug.Log("[Chest] Locked. You need a key or the Lockpick ability.");
    }

    private bool HasMatchingKey(out KeyItemData matchingKey)
    {
        matchingKey = null;

        var inventory = GetInventory();
        if (inventory == null) return false;

        foreach (var item in inventory.inventory)
        {
            if (item.data is KeyItemData keyData && keyData.keyId == requiredKeyId)
            {
                matchingKey = keyData;
                return true;
            }
        }

        return false;
    }

    private bool HasLockpick()
    {
        foreach (var slot in AbilityManager.Instance.abilities)
        {
            if (slot?.ability is LockpickAbility)
                return true;
        }
        return false;
    }

    private void ConsumeKey(KeyItemData key)
    {
        GetInventory().Remove(key);
    }

    private void Open()
    {
        _isOpen = true;

        if (objectSpawner != null)
            objectSpawner.TriggerSpawn();
        else
            Debug.LogWarning("[Chest] No ObjectSpawner assigned.");
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[Chest] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
