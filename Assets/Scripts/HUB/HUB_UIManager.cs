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
    public GameObject loadoutScreen;
    public GameObject detailsScreen;
    public Animator detailsScreenAnim;
    public GameObject readyButton;

    [Header("Debt UI")]
    public Slider debtPaidFill;
    [SerializeField] private float totalDebt;
    
    public EventSystem eventSystem;
    
    [Header ("Ability Slot UI Refs")]
    public int selectedSlot;
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
        totalDebt = GameManager.Instance.maxDebt;
        
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
                eventSystem.SetSelectedGameObject(slot1);
                CalculateDebtRemaining();
                break;
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                break;
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                eventSystem.SetSelectedGameObject(slot1);
                break;
        }
        
        
    }

    private void CalculateDebtRemaining()
    {   
        //GameManager.Instance.maxDebt -= totalDebt;
        float debtRemaining = GameManager.Instance.totalDebt;
        float normalizedDebt = debtRemaining / totalDebt;
        debtPaidFill.value = 1 - normalizedDebt;
    }

    public void SelectAbilitySlot(int slotNumber)
    {
        eventSystem.SetSelectedGameObject(firstAbility);
        cancelType = "BackToSlotSelect";
        selectedSlot = slotNumber;
    }

    public void SelectAbility()
    {
        switch (selectedSlot)
        {
            case 0:
                eventSystem.SetSelectedGameObject(slot2);
                break;
            case 1:
                eventSystem.SetSelectedGameObject(slot3);
                break;
            case 2:
                eventSystem.SetSelectedGameObject(readyButton);
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
                break;
            case "BackToSlot3":
                eventSystem.SetSelectedGameObject(slot3);
                cancelType = "BackToSlot2";
                break;
        }
        
    }
}
