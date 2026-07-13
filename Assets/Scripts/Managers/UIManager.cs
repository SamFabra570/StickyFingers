using System;
using System.Collections;
using System.Collections.Generic;
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

    public InputActionReference submitAction;
    public InputActionReference cancelAction;
    public InputActionReference buttonNorthAction;
    
    [Header ("UI Screen Refs")]
    public InventoryMenu inventoryMenu;
    public PauseMenu pauseMenu;
    public HelpMenu helpMenu;
    public LoadoutMenu loadoutMenu;
    public ProgressionMenu progressionMenu;
    public TutorialMenu tutorialMenu;
    
    public GameObject HUDCanvas;
    public Canvas HUBCanvas;
    
    [Header ("Tutorial")]
    public List<TutorialSegment> HUDTutorial = new();
    public List<Transform> HUDTutorialElements = new();
    
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
            textTotalBounty.SetText("" + inventory.totalBounty);
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

    public void StartTutorial()
    {
        if (!TutorialMenu.Instance.HasCompletedTutorial(HUDTutorial[0]))
        {
            TutorialMenu.Instance.CacheTutorialContent(HUDTutorial, HUDTutorialElements);
            OpenMenu("TutorialMenu");
        }
    }

    public void UpdateInventoryUI()
    {
        switch (PlayerController.Instance.currentState)
        {
            case PlayerController.WeightState.Light:
                weightStateText.text = ("Light");
                break;
            case PlayerController.WeightState.Medium:
                weightStateText.text = ("Moderate");
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
        else
            totalBountyText.text = ("0");
    }

    private void UpdatePassiveUI(Passive passive)
    {
        if (passive != null)
        {
            passiveBackground.color = passive.passiveColour.color;
        }
    }

    public void OpenMenu(string menu)
    {
        GameManager.Instance.PauseGame(1);
        
        InputIconManager.Instance.RefreshIcons();
        
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
            case ("TutorialMenu"):
                UIMenuStack.PushOverlay(tutorialMenu);
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
            case (TutorialMenu):
                UIMenuStack.PopOverlay();
                
                if (UIMenuStack.Current != null)
                {
                    UIMenuStack.Current.OnShowMenu();
                    return;
                }

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
            //Debug.Log("turn off text");
            return;
        }

        if (interactType == "")
        {
            if (SceneManager.GetActiveScene().name == "HUB") 
                interactText.SetText("Interact");
            else
                interactText.SetText("Steal");

            lastInteracted = null;
        }
        
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
        textTotalBounty.SetText("" + inventory.totalBounty);
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
        AbilityManager.Instance.InterruptAllAbilities();
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.PauseGame(0);
        
    }
    
    private void OnEnable()
    {
        submitAction.action.performed += OnSubmit;
        submitAction.action.Enable();
        
        cancelAction.action.performed += OnCancel;
        cancelAction.action.Enable();
        
        buttonNorthAction.action.performed += OnButtonNorth;
        buttonNorthAction.action.Enable();
    }

    private void OnDisable()
    {
        submitAction.action.performed -= OnSubmit;
        submitAction.action.Disable();
        
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
        
        buttonNorthAction.action.performed -= OnButtonNorth;
        buttonNorthAction.action.Disable();
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        UIMenuStack.Current?.OnSubmit();
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
