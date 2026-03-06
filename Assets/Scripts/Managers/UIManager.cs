using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header ("UI Screen Refs")]
    public GameObject pauseScreen;
    public GameObject inventoryScreen;

    [Header("Weight UI Refs")] 
    public Image weightFill;
    public Sprite weightFillGreen;
    public Sprite weightFillYellow;
    public Sprite weightFillRed;

    public GameObject mageSpawnNotif;
    
    [Header ("HUD UI Refs")]
    public TextMeshProUGUI textTotalWeight;
    public TextMeshProUGUI textTotalBounty;
    
    public TextMeshProUGUI textWeightPreview;
    public TextMeshProUGUI textBountyPreview;
    public GameObject objectWeightPreview;
    public GameObject objectBountyPreview;
    
    [Header ("Object Pick Up Notification Refs")]
    public GameObject objectPickupNotif;
    public GameObject objectRemoveNotif;
    public TextMeshProUGUI textNameNotif;
    public TextMeshProUGUI textRemoveNameNotif;
    public TextMeshProUGUI textWeightNotif;
    public TextMeshProUGUI textValueNotif;
    
    public Sprite emptySprite;
    
    public InventorySystem inventory;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Debug.Log(GameManager.Instance.inventorySystem.totalWeight);
        //Debug.Log(textTotalWeight.name);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            inventory = GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
            weightFill = GameObject.Find("WeightFill").GetComponent<Image>();
        }
            
        if (textTotalWeight != null) 
            textTotalWeight.SetText("Total Weight: "+inventory.totalWeight);
        if (textTotalBounty != null) 
            textTotalBounty.SetText("Total Bounty: "+inventory.totalBounty);
        if (objectPickupNotif != null) 
            objectPickupNotif.SetActive(false); 
        if (objectRemoveNotif != null) 
            objectRemoveNotif.SetActive(false); 
        if (mageSpawnNotif != null)
            mageSpawnNotif.SetActive(false);
        
        
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            UpdateWeightUI();
        }
    }

    public void UpdateWeightUI()
    {
        float normalizedWeight = inventory.totalWeight / GameManager.Instance.maxWeight;
        weightFill.fillAmount = normalizedWeight;
        
        if (normalizedWeight > 0.7f)
        {
            weightFill.sprite = weightFillRed;
        }
        else if (normalizedWeight <= 0.7f &&  normalizedWeight > 0.3f)
        {
            weightFill.sprite = weightFillYellow;
        }
        else
            weightFill.sprite = weightFillGreen;
    }

    public void ShowItemPickupNotif(InventoryItemData itemData)
    {
        StartCoroutine(ItemPickupNotif(itemData));
    }

    public void ShowItemRemoveNotif(InventoryItemData itemData)
    {
        StartCoroutine(ItemRemoveNotif(itemData));
    }
    
    public void ShowItemStolen(InventoryItemData itemData)
    {
        StartCoroutine(itemStolen(itemData));
    }

    public void ShowMageSpawnNotif()
    {
        StartCoroutine(MageSpawnNotif());
    }
    
    private IEnumerator MageSpawnNotif()
    {
        mageSpawnNotif.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        mageSpawnNotif.SetActive(false);
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
        textTotalWeight.SetText("Total Weight: "+inventory.totalWeight);
        textTotalBounty.SetText("Total Bounty: "+inventory.totalBounty);
    }
    
    public void showPreviewItem(InventoryItemData itemData)
    {
        textWeightPreview.color = Color.green;
        textWeightPreview.SetText("+ " + itemData.itemWeight);
        textBountyPreview.color = Color.green;
        textBountyPreview.SetText("+ " + itemData.itemPrice);
        objectWeightPreview.SetActive(true);
        objectBountyPreview.SetActive(true);
        
    }
    
    public void disablePreview()
    {
        objectWeightPreview.SetActive(false);
        objectBountyPreview.SetActive(false);
        
    }
    
    public IEnumerator itemStolen(InventoryItemData itemData)
    {
        textWeightPreview.color = Color.red;
        textWeightPreview.SetText("- " + itemData.itemWeight);
        textBountyPreview.color = Color.red;
        textBountyPreview.SetText("- " + itemData.itemPrice);
        objectWeightPreview.SetActive(true);
        objectBountyPreview.SetActive(true);
        
        yield return new WaitForSeconds(4f);
        
        objectWeightPreview.SetActive(false);
        objectBountyPreview.SetActive(false);
    }

    public void ShowScreen(string screenName)
    {
        if (screenName == "Pause")
        {
            if (!inventoryScreen.activeSelf)
            {
                pauseScreen.SetActive(true);
                //PlayerController.Instance.ToggleCursor();
            }
        }
        
        if (screenName == "Inventory")
        {
            if (!pauseScreen.activeSelf)
            {
                inventoryScreen.SetActive(true);
                //PlayerController.Instance.ToggleCursor();
            }
        }
            
        
        GameManager.Instance.PauseGame(1);
    }

    public void HideScreen(string screenName)
    {
        if (screenName == "Pause")
        {
            pauseScreen.SetActive(false);
            //PlayerController.Instance.ToggleCursor();
        }

        if (screenName == "Inventory")
        {
            
            inventoryScreen.SetActive(false);
            inventory.DeselectAllSlots();
            inventory.itemDescriptionNameText.SetText("");
            inventory.itemDescriptionText.SetText("");
            inventory.itemDescriptionImage.sprite = emptySprite;
            //PlayerController.Instance.ToggleCursor();
        }
        
        GameManager.Instance.PauseGame(0);
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.PauseGame(0);
    }
}
