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

    private PlayerController player;
    
    public InputActionReference cancelAction;
    public InputActionReference buttonNorthAction;
    
    [Header ("UI Screen Refs")]
    public InventoryMenu inventoryMenu;
    public PauseMenu pauseMenu;
    public HelpMenu helpMenu;
    public LoadoutMenu loadoutMenu;
    public ProgressionMenu progressionMenu;
    
    public GameObject HUDCanvas;
    public Canvas HUBCanvas;
    
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
    public GameObject openInventoryText;

    private String lastInteracted;

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
    public PopupUI objectPopupUI;
    // public GameObject objectDropUI;
    // public TextMeshProUGUI textNameNotif;
    // public TextMeshProUGUI textRemoveNameNotif;
    // public TextMeshProUGUI textWeightNotif;
    // public TextMeshProUGUI textValueNotif;
    
    // public Sprite emptySprite;
    
    public InventorySystem inventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            inventory = GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
            weightFill = GameObject.Find("WeightFill").GetComponent<Image>();
            UpdatePassiveUI(GameManager.Instance.PlayerPassives.equippedPassive);
        }
        
        player = PlayerController.Instance;
            
        objectPopupUI = player.itemPickupUI;
            
        if (textTotalWeight != null) 
            textTotalWeight.SetText("Total Weight: "+inventory.totalWeight);
        if (textTotalBounty != null) 
            textTotalBounty.SetText("Total Bounty: "+inventory.totalBounty);
        if (objectPopupUI != null) 
            objectPopupUI.gameObject.SetActive(false); 
        if (mageSpawnNotif != null)
            mageSpawnNotif.SetActive(false);
        if (portalSpawnNotif != null)
            portalSpawnNotif.SetActive(false);
        
        
        if (interactText == null)
            interactText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
        interactText.gameObject.SetActive(false);
        
        UIMenuStack.Clear();
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

    public void UpdateInventoryUI()
    {
        switch (PlayerController.Instance.currentState)
        {
            case PlayerController.WeightState.Light:
                weightStateText.text = ("Light");
                break;
            case PlayerController.WeightState.Medium:
                weightStateText.text = ("Medium");
                break;
            case PlayerController.WeightState.Heavy:
                weightStateText.text = ("Heavy");
                break;
            case PlayerController.WeightState.Overweight:
                weightStateText.text = ("Overweight");
                break;
        }
        
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

    public void OpenMenu(string menu)
    {
        GameManager.Instance.PauseGame(1);
        
        if (openInventoryText != null) 
            openInventoryText.SetActive(false);

        switch (menu)
        {
            case ("InventoryMenu"):
                UIMenuStack.Push(inventoryMenu);
                break;
            case ("PauseMenu"):
                UIMenuStack.Push(pauseMenu);
                break;
            case ("HelpMenu"):
                UIMenuStack.Push(helpMenu);
                break;
            case ("LoadoutMenu"):
                UIMenuStack.Push(loadoutMenu);
                break;
            case ("ProgressionMenu"):
                UIMenuStack.Push(progressionMenu);
                break;
        }
    }

    public void HideMenu()
    {
        if (openInventoryText != null) 
            openInventoryText.SetActive(true);

        switch (UIMenuStack.Current)
        {
            case (InventoryMenu):
                break;
            case (PauseMenu):
                break;
            case (HelpMenu):
                UIMenuStack.Pop();
                return;
            case (LoadoutMenu):
                break;
            case (ProgressionMenu):
                break;
        }
        
        UIMenuStack.Clear();
        
        GameManager.Instance.PauseGame(0);
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
        
        if (interactType == "") 
            lastInteracted = interactType;

        switch (lastInteracted)
        {
            case "Object":
                interactText.SetText("Steal");
                break;
            case "Interactable":
                interactText.SetText("Interact");
                break;
            case "MashEvent":
                interactText.SetText("Mash");
                break;
        }
        
        interactText.gameObject.SetActive(true);
    }

    public void ShowItemPopupUI(InventoryItemData itemData, PopupUI.PopupType popupType)
    {
        PopupUI.Instance.SetPopupContent(itemData, popupType);
        StartCoroutine(ItemPickupNotif(itemData));
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
        objectPopupUI.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        objectPopupUI.gameObject.SetActive(false);
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
        switch (PlayerController.Instance.currentState)
        {
            case PlayerController.WeightState.Light:
                weightFill.sprite = weightFillGreen;
                weightFillInv.sprite = weightFillGreen;
                break;
            case PlayerController.WeightState.Medium:
                weightFill.sprite = weightFillYellow;
                weightFillInv.sprite = weightFillYellow;
                break;
            case PlayerController.WeightState.Heavy:
                weightFill.sprite = weightFillRed;
                weightFillInv.sprite = weightFillRed;
                break;
        }
    }
    
    public void ShowPreviewItem(InventoryItemData itemData)
    {
        float previewWeight = inventory.totalWeight + itemData.itemWeight;
        float normalizedWeightPreview = previewWeight / GameManager.Instance.maxWeight;
        weightPreviewFill.fillAmount = normalizedWeightPreview;
        weightPreviewFillInv.fillAmount = normalizedWeightPreview;
        
        textWeightPreview.color = Color.forestGreen;
        textWeightPreview.SetText("+ " + itemData.itemWeight);
        textBountyPreview.color = Color.forestGreen;
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
        textWeightPreview.color = Color.darkRed;
        textWeightPreview.SetText("- " + itemData.itemWeight);
        textBountyPreview.color = Color.darkRed;
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
    
    private void OnEnable()
    {
        cancelAction.action.performed += OnCancel;
        cancelAction.action.Enable();
        
        buttonNorthAction.action.performed += OnButtonNorth;
        buttonNorthAction.action.Enable();
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
        
        buttonNorthAction.action.performed -= OnButtonNorth;
        buttonNorthAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        UIMenuStack.Current?.OnCancel();
    }

    private void OnButtonNorth(InputAction.CallbackContext context)
    {
        UIMenuStack.Current?.OnButtonNorth();
    }
}
