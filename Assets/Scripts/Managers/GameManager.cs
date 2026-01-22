using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    /*public InventorySystem inventorySystem;
    public InventoryItemData item1;
    public InventoryItemData item2;*/

    private void Awake()
    {
        Instance = this;
        /*Instance.inventorySystem = new InventorySystem();
        inventorySystem.Add(item1);
        inventorySystem.Add(item2);*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame(int pauseState)
    {
        if (pauseState == 0)
        {
            Time.timeScale = 1;
        }
        
        if (pauseState == 1)
        {
            Time.timeScale = 0;
        }
    }
}
