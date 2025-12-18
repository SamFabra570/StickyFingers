using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pauseScreen;
    public GameObject inventoryScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen(string screenName)
    {
        if (screenName == "Pause")
        {
            if (!inventoryScreen.activeSelf)
            {
                pauseScreen.SetActive(true);
            }
        }
        
        if (screenName == "Inventory")
        {
            if (!pauseScreen.activeSelf)
            {
                inventoryScreen.SetActive(true);
            }
        }
            
        
        GameManager.Instance.PauseGame(1);
    }

    public void HideScreen(string screenName)
    {
        if (screenName == "Pause") 
            pauseScreen.SetActive(false);
        if (screenName == "Inventory")
            inventoryScreen.SetActive(false);
        
        GameManager.Instance.PauseGame(0);
    }
}
