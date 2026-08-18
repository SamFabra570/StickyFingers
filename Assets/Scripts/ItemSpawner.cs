using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private Transform levelParent;

    [Header("Spawn Settings")]
    [Tooltip("Total number of items to spawn across the entire level.")]
    [SerializeField] private int totalItemsToSpawn = 20;

    [Tooltip("Minimum relative weight used when distributing items between rooms.")]
    [SerializeField, Range(0f, 1f)]
    private float minRoomDensity = 0.15f;

    [Tooltip("Maximum relative weight used when distributing items between rooms.")]
    [SerializeField, Range(0f, 1f)]
    private float maxRoomDensity = 0.30f;

    [Header("Item Rarity")]
    [SerializeField, Range(0f, 1f)]
    private float commonSpawnChance = 0.60f;

    [SerializeField, Range(0f, 1f)]
    private float uncommonSpawnChance = 0.30f;

    [SerializeField, Range(0f, 1f)]
    private float rareSpawnChance = 0.10f;

    [Header("Spawnable Items")]
    [SerializeField] private List<InventoryItemData> spawnableItems = new();

    [Header("Spawned Items")]
    [SerializeField] private Transform spawnedItemParent;

    [SerializeField] private Vector3 spawnRotation = new (-90f, 0f, 0f);

    private readonly Dictionary<Transform, List<Transform>> roomSpawnPoints = new();
    private readonly List<GameObject> spawnedItems = new();

    private void Awake()
    {
        FindRooms();
    }
    
    private void Start()
    {
        SpawnItems();
    }

    private void FindRooms()
    {
        roomSpawnPoints.Clear();

        foreach (Transform child in levelParent)
        {
            if (!child.CompareTag("Room"))
                continue;

            List<Transform> spawnPoints = new();

            foreach (Transform roomChild in child)
            {
                if (roomChild.name != "ItemSpawnPoints")
                    continue;

                foreach (Transform spawnPoint in roomChild)
                {
                    if (spawnPoint.CompareTag("ItemSpawnPoint"))
                    {
                        spawnPoints.Add(spawnPoint);
                    }
                }
            }

            roomSpawnPoints.Add(child, spawnPoints);
        }

        Debug.Log($"Found {roomSpawnPoints.Count} rooms.");

        foreach (var room in roomSpawnPoints)
        {
            Debug.Log($"{room.Key.name}: {room.Value.Count} spawn points");
        }
    }
    
    private void SpawnItems()
    {
        if (roomSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No rooms found.");
            return;
        }

        if (spawnableItems.Count == 0)
        {
            Debug.LogWarning("No spawnable items assigned.");
            return;
        }

        Dictionary<Transform, int> roomItemAmounts = CalculateRoomDistribution(totalItemsToSpawn);

        foreach (var room in roomItemAmounts)
        {
            List<Transform> points = new(roomSpawnPoints[room.Key]);

            Shuffle(points);

            int amount = room.Value;

            for (int i = 0; i < amount; i++)
            {
                SpawnItem(points[i]);
            }

            Debug.Log($"{room.Key.name}: spawned {amount} items.");
        }
    }
    
    private Dictionary<Transform, int> CalculateRoomDistribution(
        int totalAmount)
    {
        Dictionary<Transform, int> roomItemAmounts = new();
        Dictionary<Transform, float> roomDensities = new();

        int remainingItems = totalAmount;

        foreach (var room in roomSpawnPoints)
        {
            float density = Random.Range(minRoomDensity, maxRoomDensity);

            roomDensities.Add(room.Key, density);
        }

        float totalDensity = 0f;

        foreach (float density in roomDensities.Values)
        {
            totalDensity += density;
        }

        foreach (var room in roomDensities)
        {
            float roomShare = room.Value / totalDensity;

            int amount = Mathf.FloorToInt(totalAmount * roomShare);

            amount = Mathf.Min(amount, roomSpawnPoints[room.Key].Count);

            roomItemAmounts.Add(room.Key, amount);

            remainingItems -= amount;
        }

        // Distribute leftover items while there are still
        // available spawn points.
        while (remainingItems > 0)
        {
            bool itemAdded = false;

            foreach (var room in roomSpawnPoints)
            {
                if (remainingItems <= 0)
                    break;

                int currentAmount = roomItemAmounts[room.Key];

                if (currentAmount >= room.Value.Count)
                    continue;

                roomItemAmounts[room.Key]++;
                remainingItems--;

                itemAdded = true;
            }

            if (!itemAdded)
            {
                Debug.LogWarning("Not enough spawn points to spawn the requested number of items.");
                break;
            }
        }

        return roomItemAmounts;
    }
    
    private void SpawnItem(Transform spawnPoint)
    {
        ItemRarity rarity = GetRandomRarity();

        InventoryItemData item = GetRandomItemByRarity(rarity);

        if (item == null)
        {
            Debug.LogWarning($"No item found for rarity: {rarity}");
            return;
        }

        if (item.prefab == null)
        {
            Debug.LogWarning($"Item '{item.itemName}' does not have a prefab assigned.");
            return;
        }

        GameObject spawnedItem = Instantiate(item.prefab, spawnPoint.position, Quaternion.Euler(spawnRotation), spawnedItemParent);

        spawnedItems.Add(spawnedItem);
    }

    private ItemRarity GetRandomRarity()
    {
        float roll = Random.value;

        if (roll < commonSpawnChance) return ItemRarity.Common;

        if (roll < commonSpawnChance + uncommonSpawnChance) return ItemRarity.Uncommon;

        return ItemRarity.Rare;
    }

    private InventoryItemData GetRandomItemByRarity(
        ItemRarity rarity)
    {
        List<InventoryItemData> matchingItems = new();

        foreach (InventoryItemData item in spawnableItems)
        {
            if (item == null)
                continue;

            if (item.itemRarity == rarity)
            {
                matchingItems.Add(item);
            }
        }

        if (matchingItems.Count == 0)
            return null;

        int randomIndex = Random.Range(0, matchingItems.Count);

        return matchingItems[randomIndex];
    }
    
    private void Shuffle(List<Transform> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    public void ClearSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedItems.Clear();
    }
}
