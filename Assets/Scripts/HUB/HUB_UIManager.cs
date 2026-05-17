using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUB_UIManager : MonoBehaviour
{
    public static HUB_UIManager Instance;
    
    public InputActionReference cancelAction;

    [Header("Planning UI Refs")]
    public Canvas planningUI;
    public GameObject detailsScreen;
    public Animator detailsScreenAnim;
    public GameObject readyButton;
    public GameObject passiveAbilitySlot;

    [Header("Progression UI Refs")] 
    public GameObject progressionScreen;

    [Header("Debt UI")]
    public Slider debtPaidFill;
    
    public EventSystem eventSystem;
    
    [Header ("Ability Slot UI Refs")]
    public GameObject selectedSlot;
    public GameObject selectedAbility;
    
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    
    public GameObject firstAbility;

    private string cancelType = "Exit";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TogglePlanningUI("Close");
    }

    public void TogglePlanningUI(string status)
    {
        PlayerController.Instance.inputMap.UI.Enable();
        PlayerController.Instance.inputMap.Player.Disable();
        
        switch (status)
        {
            case "Close":
                planningUI.enabled = false;
                if (PlayerController.Instance.isFrozen)
                    PlayerController.Instance.isFrozen = false;
                
                PlayerController.Instance.inputMap.UI.Disable();
                PlayerController.Instance.inputMap.Player.Enable();
                break;
            case "Show":
                planningUI.enabled = true;
                if (progressionScreen.activeSelf)
                    progressionScreen.SetActive(false);
                
                if (!detailsScreen.activeSelf)
                    detailsScreen.SetActive(true);
                
                eventSystem.SetSelectedGameObject(slot1);
                debtPaidFill.value = GameManager.Instance.GetDebtPaidPercent();
                break;
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                break;
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                eventSystem.SetSelectedGameObject(slot1);
                break;
            case "Progression":
                planningUI.enabled = true;
                if (!progressionScreen.activeSelf)
                    progressionScreen.SetActive(true);
                
                if (detailsScreen.activeSelf)
                    detailsScreen.SetActive(false);
                break;
        }
    }

    public void SelectAbilitySlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;
        
        eventSystem.SetSelectedGameObject(firstAbility);
        cancelType = "BackToSlotSelect";
    }

    //Select ability using controller
    public void SelectAbility()
    {
        selectedAbility =  eventSystem.currentSelectedGameObject;
        DraggableItem slot = selectedAbility.GetComponentInChildren<DraggableItem>();

        if (selectedSlot == null)
            return;
        
        switch (selectedSlot.name)
        {
            case "AbilitySlot1":
                AbilityManager.Instance.EquipAbility(0, slot.ability);
                Debug.Log("Ability 1 set" + slot.ability);
                
                eventSystem.SetSelectedGameObject(slot2);
                break;
            case "AbilitySlot2":
                AbilityManager.Instance.EquipAbility(1, slot.ability);
                Debug.Log("Ability 2 set" + slot.ability);
                
                eventSystem.SetSelectedGameObject(slot3);
                break;
            case "AbilitySlot3":
                AbilityManager.Instance.EquipAbility(2, slot.ability);
                Debug.Log("Ability 3 set" + slot.ability);
                
                eventSystem.SetSelectedGameObject(passiveAbilitySlot);
                cancelType = "BackToSlot3";
                break;
            case "PassiveAbilitySlot":
                //Add equip passive ability logic here
                
                eventSystem.SetSelectedGameObject(readyButton);
                cancelType = "BackToPassiveAbilitySlot";
                break;
        }
        
        slot.transform.SetParent(selectedSlot.transform);
        
        selectedSlot = null;
    }
    
    private void OnCancel(InputAction.CallbackContext context)
    {
        switch (cancelType)
        {
            case "Exit":
                TogglePlanningUI("Close");
                break;
            case "BackToSlotSelect":
                eventSystem.SetSelectedGameObject(slot1);
                cancelType = "Exit";
                break;
            case "BackToSlot2":
                eventSystem.SetSelectedGameObject(slot2);
                cancelType = "BackToSlotSelect";
                break;
            case "BackToSlot3":
                eventSystem.SetSelectedGameObject(slot3);
                cancelType = "BackToSlot2";
                break;
            case "BackToPassiveAbilitySlot":
                eventSystem.SetSelectedGameObject(passiveAbilitySlot);
                cancelType = "BackToSlot3";
                break;
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
}
