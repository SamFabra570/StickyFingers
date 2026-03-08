using System;
using UnityEngine;

public class MageSpawner : MonoBehaviour
{
    public static  MageSpawner Instance;
    
    public Transform mageSpawn;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject mage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SpawnMage()
    {
        GameObject prefab = Resources.Load<GameObject>("Mage");
        mage = Instantiate(prefab);
        mage.transform.position = mageSpawn.position;
        
        UIManager.Instance.ShowMageSpawnNotif();
    }
}
