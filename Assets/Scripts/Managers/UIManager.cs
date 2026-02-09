using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pauseScreen;
    public GameObject inventoryScreen;
    
    public TextMeshProUGUI textTotalWeight;
    public TextMeshProUGUI textTotalBounty;

    public GameObject objectPickupNotif;
    public GameObject objectRemoveNotif;
    public TextMeshProUGUI textNameNotif;
    public TextMeshProUGUI textRemoveNameNotif;
    public TextMeshProUGUI textWeightNotif;
    public TextMeshProUGUI textValueNotif;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Debug.Log(GameManager.Instance.inventorySystem.totalWeight);
        //Debug.Log(textTotalWeight.name);
        textTotalWeight.SetText("Total Weight: "+GameManager.Instance.inventorySystem.totalWeight);
        textTotalBounty.SetText("Total Bounty: "+GameManager.Instance.inventorySystem.totalBounty);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        objectPickupNotif.SetActive(false);
        objectRemoveNotif.SetActive(false);
    }

    public void ShowItemPickupNotif(InventoryItemData itemData)
    {
        StartCoroutine(ItemPickupNotif(itemData));
    }

    public void ShowItemRemoveNotif(InventoryItemData itemData)
    {
        StartCoroutine(ItemRemoveNotif(itemData));
    }

    private IEnumerator ItemPickupNotif(InventoryItemData itemData)
    {
        textNameNotif.SetText(itemData.itemName);
        textWeightNotif.SetText("Weight: " + itemData.itemWeight);
        textValueNotif.SetText("Value: " + itemData.itemPrice);
        objectPickupNotif.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        objectPickupNotif.SetActive(false);
    }
    
    private IEnumerator ItemRemoveNotif(InventoryItemData itemData)
    {
        textRemoveNameNotif.SetText("- " + itemData.itemName);
        objectRemoveNotif.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        objectRemoveNotif.SetActive(false);
    }

    public void UpdateTotals()
    {
        textTotalWeight.SetText("Total Weight: "+GameManager.Instance.inventorySystem.totalWeight);
        textTotalBounty.SetText("Total Bounty: "+GameManager.Instance.inventorySystem.totalBounty);
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
    
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.PauseGame(0);
    }
}
