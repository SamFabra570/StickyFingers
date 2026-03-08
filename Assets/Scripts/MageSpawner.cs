using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MageSpawner : MonoBehaviour
{
    public static  MageSpawner Instance;
    
    public Transform mageSpawn;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject mage;
    public List<Transform> waypoints;

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
        mage.GetComponent<BaseMageEnemy>().waypoints = waypoints;
        mage.transform.position = mageSpawn.position;
        
        UIManager.Instance.ShowMageSpawnNotif();
    }
}
