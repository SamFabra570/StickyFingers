using UnityEngine;
using ZoneInteractables;

public class DiningTable : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform[] plateSlots;
    [SerializeField] private ObjectSpawner keySpawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bellSound;

    private int _platesPlaced = 0;
    private bool _completed = false;

    public void Interact(GameObject player)
    {
        if (_completed) return;

        var inventory = GetInventory();
        if (inventory == null) return;

        PlateItemData plate = FindPlateInInventory(inventory);
        if (plate == null)
        {
            Debug.Log("[DiningTable] No plate in inventory.");
            return;
        }

        if (_platesPlaced >= plateSlots.Length)
        {
            Debug.Log("[DiningTable] All slots are full.");
            return;
        }

        inventory.Remove(plate);
        PlacePlate(plate);
    }

    private void PlacePlate(PlateItemData plate)
    {
        Transform slot = plateSlots[_platesPlaced];

        if (plate.prefab != null)
            Instantiate(plate.prefab, slot.position, slot.rotation);

        _platesPlaced++;

        if (_platesPlaced >= plateSlots.Length)
            Complete();
    }

    private void Complete()
    {
        _completed = true;

        if (audioSource != null && bellSound != null)
            audioSource.PlayOneShot(bellSound);

        if (keySpawner != null)
            keySpawner.TriggerSpawn();
        else
            Debug.LogWarning("[DiningTable] No key spawner assigned.");
    }

    private PlateItemData FindPlateInInventory(InventorySystem inventory)
    {
        foreach (var item in inventory.inventory)
        {
            if (item.data is PlateItemData plate)
                return plate;
        }
        return null;
    }

    private InventorySystem GetInventory()
    {
        var container = GameObject.Find("InventoryContainer")?.GetComponent<InventoryContainer>();
        if (container == null)
        {
            Debug.LogWarning("[DiningTable] InventoryContainer not found.");
            return null;
        }
        return container.inventorySystem;
    }
}
