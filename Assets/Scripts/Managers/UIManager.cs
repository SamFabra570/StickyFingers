using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public InputActionReference cancelAction;
    
    [Header ("UI Screen Refs")]
    public GameObject pauseScreen;
    public GameObject inventoryScreen;
    
    [SerializeField] private GameObject pauseMenuFirstButton;
    [SerializeField] private GameObject invMenuFirstObject;

    private EventSystem eventSystem;

    [Header("Weight UI Refs")] 
    public Image weightFill;
    public Image weightPreviewFill;
    public Sprite weightFillGreen;
    public Sprite weightFillYellow;
    public Sprite weightFillRed;

    [Header ("Mage UI Refs")]
    public GameObject mageSpawnNotif;

    [Header("HUD UI Refs")] 
    public TextMeshProUGUI interactText;

    public GameObject portalSpawnNotif;
    
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
        
        eventSystem = EventSystem.current;
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
        if (portalSpawnNotif != null)
            portalSpawnNotif.SetActive(false);
        
        
        if (interactText != null)
        {
            interactText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
            interactText.gameObject.SetActive(false);
        }
        
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

    private void OnEnable()
    {
        cancelAction.action.performed += OnCancel;
        cancelAction.action.Enable();
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (pauseScreen.activeSelf)
            HideScreen("Pause");
        if (inventoryScreen.activeSelf)
            HideScreen("Inventory");
        
    }

    public void ShowScreen(string screenName)
    {
        if (screenName == "Pause")
        {
            if (!inventoryScreen.activeSelf)
            {
                pauseScreen.SetActive(true);
                eventSystem.SetSelectedGameObject(pauseMenuFirstButton);
                //PlayerController.Instance.ToggleCursor();
            }
        }
        
        if (screenName == "Inventory")
        {
            if (!pauseScreen.activeSelf)
            {
                inventoryScreen.SetActive(true);
                //eventSystem.SetSelectedGameObject(invMenuFirstObject);
                //PlayerController.Instance.ToggleCursor();
            }
        }
        
        PlayerController.Instance.inputMap.UI.Enable();
        PlayerController.Instance.inputMap.Player.Disable();
        
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        
        GameManager.Instance.PauseGame(1);
        PlayerController.Instance.isPaused = true;
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
        
        PlayerController.Instance.inputMap.UI.Disable();
        PlayerController.Instance.inputMap.Player.Enable();
        
        GameManager.Instance.PauseGame(0);
        PlayerController.Instance.isPaused = false;
    }

    public void ToggleInteractText(bool showText, string interactType)
    {
        if (!showText)
        {
            interactText.gameObject.SetActive(false);
            return;
        }

        switch (interactType)
        {
            case "Object":
                interactText.SetText("Press 'F' to steal");
                break;
            case "Interactable":
                interactText.SetText("Press 'F' to interact");
                break;
            case "MashEvent":
                interactText.SetText("Press 'F' to mash");
                break;
        }
        
        interactText.gameObject.SetActive(true);
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
        StartCoroutine(ItemStolen(itemData));
    }

    public void ShowMageSpawnNotif()
    {
        StartCoroutine(MageSpawnNotif());
    }

    public void ShowPortalSpawnNotif()
    {
        StartCoroutine(PortalSpawnNotif());
    }
    
    private IEnumerator MageSpawnNotif()
    {
        mageSpawnNotif.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        mageSpawnNotif.SetActive(false);
    }
    
    private IEnumerator PortalSpawnNotif()
    {
        portalSpawnNotif.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        portalSpawnNotif.SetActive(false);
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
    
    private void UpdateWeightUI()
    {
        float normalizedWeight = inventory.totalWeight / GameManager.Instance.maxWeight;
        weightFill.fillAmount = normalizedWeight;
        
        //Change UI colour based on weight
        if (normalizedWeight > 0.66f)
        {
            weightFill.sprite = weightFillRed;
        }
        else if (normalizedWeight <= 0.66f &&  normalizedWeight > 0.3f)
        {
            weightFill.sprite = weightFillYellow;
        }
        else
            weightFill.sprite = weightFillGreen;
    }
    
    public void ShowPreviewItem(InventoryItemData itemData)
    {
        float previewWeight = inventory.totalWeight + itemData.itemWeight;
        float normalizedWeightPreview = previewWeight / GameManager.Instance.maxWeight;
        weightPreviewFill.fillAmount = normalizedWeightPreview;
        
        textWeightPreview.color = Color.green;
        textWeightPreview.SetText("+ " + itemData.itemWeight);
        textBountyPreview.color = Color.green;
        textBountyPreview.SetText("+ " + itemData.itemPrice);
        objectWeightPreview.SetActive(true);
        objectBountyPreview.SetActive(true);
        
    }
    
    public void DisablePreview()
    {
        weightPreviewFill.fillAmount = 0f;
        
        objectWeightPreview.SetActive(false);
        objectBountyPreview.SetActive(false);
        
    }
    
    private IEnumerator ItemStolen(InventoryItemData itemData)
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
    
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.PauseGame(0);
    }
}
