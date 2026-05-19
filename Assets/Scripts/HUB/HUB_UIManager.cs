using System;
using TMPro;
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

    [Header("Progression UI Refs")] 
    public GameObject progressionScreen;

    [Header("Debt UI")]
    public Slider debtPaidFill;
    public TextMeshProUGUI debtPaidText;
    public TextMeshProUGUI totalDebtText;
    
    public EventSystem eventSystem;

    [Header ("Loadout Menu")]
    public LoadoutMenu loadoutMenu;
    
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
        progressionScreen.SetActive(false);
        planningUI.enabled = false;
                
        if (PlayerController.Instance.isFrozen)
            PlayerController.Instance.isFrozen = false;
                
        PlayerController.Instance.inputMap.UI.Disable();
        PlayerController.Instance.inputMap.Player.Enable();
                
        UIMenuStack.Clear();
        
        
    }

    public void TogglePlanningUI(string status)
    {
        switch (status)
        {
            case "Close":
                planningUI.enabled = false;
                UIManager.Instance.ToggleInteractText(true, "");
                
                if (PlayerController.Instance.isFrozen)
                    PlayerController.Instance.isFrozen = false;
                
                PlayerController.Instance.inputMap.UI.Disable();
                PlayerController.Instance.inputMap.Player.Enable();
                
                UIMenuStack.Clear();
                break;
            
            case "Show":
                UIManager.Instance.ToggleInteractText(false, "");
                planningUI.enabled = true;
                
                if (progressionScreen.activeSelf)
                    progressionScreen.SetActive(false);
                
                if (!detailsScreen.activeSelf)
                    detailsScreen.SetActive(true);
                
                eventSystem.SetSelectedGameObject(loadoutMenu.slot1);
                
                UpdateAbilityLockState();
                
                debtPaidFill.value = GameManager.Instance.GetDebtPaidPercent();
                debtPaidText.text = ("" + (GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt));
                totalDebtText.text = ("" + GameManager.Instance.maxDebt);
                
                PlayerController.Instance.inputMap.UI.Enable();
                PlayerController.Instance.inputMap.Player.Disable();
                break;
            
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                break;
            
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                
                UIMenuStack.Push(loadoutMenu);
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

    private void UpdateAbilityLockState()
    {
        var abilityUIs = progressionScreen.GetComponentsInChildren<AbilityUnlock>(true);

        foreach (var ui in abilityUIs)
        {
            ui.UpdateState();
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        
    }
    
    private void OnCancel(InputAction.CallbackContext context)
    {
        if (UIMenuStack.Current != null)
            UIMenuStack.Pop();
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
