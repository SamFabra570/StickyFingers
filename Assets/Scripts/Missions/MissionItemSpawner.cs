using System.Collections.Generic;
using UnityEngine;

public class MissionItemSpawner : MonoBehaviour
{
    [Header ("Spawn Settings")]
    [SerializeField] private Transform spawnPointParent;
    [SerializeField] private Transform missionItemParent;
    
    [SerializeField] private int minSpawnAmount = 2;
    
    [SerializeField] private float minSpawnPercent = 0.3f;
    [SerializeField] private float maxSpawnPercent = 0.5f;
    
    private readonly List<Transform> spawnPoints = new();
    private readonly List<GameObject> spawnedItems = new();

    private void Awake()
    {
        GetSpawnPoints();
    }

    private void Start()
    {
        if (MissionManager.Instance.activeMission != null)
        {
            SpawnMissionItems(MissionManager.Instance.activeMission);
        }
    }

    private void GetSpawnPoints()
    {
        spawnPoints.Clear();

        foreach (Transform child in spawnPointParent)
        {
            spawnPoints.Add(child);
        }
    }

    private void SpawnMissionItems(MissionData mission)
    {
        ClearMissionItems();

        if (mission == null)
            return;

        int remaining = mission.requiredAmount - mission.currentAmount;

        if (remaining <= 0)
            return;

        float randomPercent = Random.Range(minSpawnPercent, maxSpawnPercent);

        int spawnAmount = Mathf.CeilToInt(remaining * randomPercent);

        spawnAmount = Mathf.Clamp(spawnAmount, minSpawnAmount, spawnPoints.Count);

        //Never spawn more than the player actually needs
        spawnAmount = Mathf.Min(spawnAmount, remaining);

        ShuffleSpawnPoints();

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject item = Instantiate(mission.itemPrefab, spawnPoints[i].position, Quaternion.Euler(-90f, 0f, 0f), missionItemParent);

            spawnedItems.Add(item);
        }
    }
    
    private void ClearMissionItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedItems.Clear();
    }
    
    private void ShuffleSpawnPoints()
    {
        for (int i = spawnPoints.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (spawnPoints[i], spawnPoints[randomIndex]) =
                (spawnPoints[randomIndex], spawnPoints[i]);
        }
    }
}
