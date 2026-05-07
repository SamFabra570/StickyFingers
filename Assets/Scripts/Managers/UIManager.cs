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
    public GameObject HUDCanvas;
    
    [SerializeField] private GameObject pauseMenuFirstButton;
    [SerializeField] private GameObject invMenuFirstObject;

    private EventSystem eventSystem;
    
    [Header("Inventory UI Refs")]
    public Image weightFillInv;
    public Image weightPreviewFillInv;
    public TextMeshProUGUI weightStateText;
    public TextMeshProUGUI totalWeightText;
    public TextMeshProUGUI maxWeightText;

    public TextMeshProUGUI totalBountyText;

    public Image passiveBackground;
    public Image passiveIcon;

    [Header("Weight HUD Refs")] 
    public float normalizedWeight;
    
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

    [Header("Object Trigger UI Refs")] 
    public GameObject triggeredObject;

    public Slider mashBar;
    public bool isMashing;
    private ButtonMash mashScript;
    
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
            UpdatePassiveUI(GameManager.Instance.PlayerPassives.equippedPassive);
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
            UpdateInventoryUI();
        }
        
        if (isMashing)
            UpdateMashBar();
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

    private void UpdateInventoryUI()
    {
        maxWeightText.text = ("" + GameManager.Instance.maxWeight);
        totalWeightText.text = (inventory.totalWeight + " /");

        if (inventory.totalBounty > 0) 
            totalBountyText.text = ("" + inventory.totalBounty);
    }

    private void UpdatePassiveUI(PassivesUIData passive)
    {
        if (passive != null)
        {
            if (passive.icon != null) 
                passiveIcon.sprite = passive.icon;
            
            passiveBackground.color = passive.frameColor;
        }
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
            if (SceneManager.GetActiveScene().name != "Game")
                return;
            
            if (!pauseScreen.activeSelf)
            {
                inventoryScreen.SetActive(true);
                HUDCanvas.SetActive(false);
                //eventSystem.SetSelectedGameObject(invMenuFirstObject);
                //PlayerController.Instance.ToggleCursor();
            }
        }
        
        //PlayerController.Instance.inputMap.UI.Enable();
        //PlayerController.Instance.inputMap.Player.Disable();
        
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
            HUDCanvas.SetActive(true);
            
            if (SceneManager.GetActiveScene().name == "Game")
            {
                inventory.DeselectAllSlots();
                inventory.itemDescriptionNameText.SetText("");
                inventory.itemDescriptionText.SetText("");
                inventory.itemDescriptionImage.sprite = emptySprite;
            }
            //PlayerController.Instance.ToggleCursor();
        }
        
        //PlayerController.Instance.inputMap.UI.Disable();
        //PlayerController.Instance.inputMap.Player.Enable();
        
        GameManager.Instance.PauseGame(0);
        PlayerController.Instance.isPaused = false;
    }
    
    public void SetTriggeredObject(GameObject objectTriggered)
    {
        if (objectTriggered.GetComponent<ButtonMash>())
        {
            mashScript = objectTriggered.GetComponent<ButtonMash>();
            isMashing = true;
        }
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
    
    private void UpdateMashBar()
    {
        float normalizedTimeRemaining = mashScript.timeRemaining / mashScript.maxEventTime;
        mashBar.value = normalizedTimeRemaining;
    }

    public void UpdateTotals()
    {
        textTotalWeight.SetText("Total Weight: "+inventory.totalWeight);
        textTotalBounty.SetText("Total Bounty: "+inventory.totalBounty);
    }
    
    private void UpdateWeightUI()
    {
        normalizedWeight = inventory.totalWeight / GameManager.Instance.maxWeight;
        weightFill.fillAmount = normalizedWeight;
        weightFillInv.fillAmount = normalizedWeight;
        
        //Change UI colour based on weight
        if (normalizedWeight > 0.66f)
        {
            weightFill.sprite = weightFillRed;
            weightFillInv.sprite = weightFillRed;
        }
        else if (normalizedWeight <= 0.66f &&  normalizedWeight > 0.3f)
        {
            weightFill.sprite = weightFillYellow;
            weightFillInv.sprite = weightFillYellow;
        }
        else
        {
            weightFill.sprite = weightFillGreen;
            weightFillInv.sprite = weightFillGreen;
        }
            
    }
    
    public void ShowPreviewItem(InventoryItemData itemData)
    {
        float previewWeight = inventory.totalWeight + itemData.itemWeight;
        float normalizedWeightPreview = previewWeight / GameManager.Instance.maxWeight;
        weightPreviewFill.fillAmount = normalizedWeightPreview;
        weightPreviewFillInv.fillAmount = normalizedWeightPreview;
        
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
        weightPreviewFillInv.fillAmount = 0f;
        
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
