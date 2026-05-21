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
    public GameObject loadoutScreen;
    
    public Animator detailsScreenAnim;

    [Header("Progression UI Refs")] 
    public GameObject progressionScreen;

    [Header("Debt UI")]
    public Slider debtPaidFill;
    public TextMeshProUGUI debtPaidText;
    public TextMeshProUGUI totalDebtText;
    
    public EventSystem eventSystem;

    [Header ("Menu Refs")]
    public LoadoutMenu loadoutMenu;
    public ProgressionMenu progressionMenu;
    
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
        UIMenuStack.Clear();
        planningUI.enabled = false;
                
        if (PlayerController.Instance.isFrozen)
            PlayerController.Instance.isFrozen = false;
                
        PlayerController.Instance.inputMap.UI.Disable();
        PlayerController.Instance.inputMap.Player.Enable();
    }

    public void TogglePlanningUI(string status)
    {
        UpdateAbilityLockState();
        
        debtPaidFill.value = GameManager.Instance.GetDebtPaidPercent();
        debtPaidText.text = ("" + (GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt));
        totalDebtText.text = ("" + GameManager.Instance.maxDebt);
        
        switch (status)
        {
            case "Close":
                UIMenuStack.Clear();
                planningUI.enabled = false;
                
                UIManager.Instance.ToggleInteractText(true, "");
                
                if (PlayerController.Instance.isFrozen)
                    PlayerController.Instance.isFrozen = false;
                
                PlayerController.Instance.inputMap.UI.Disable();
                PlayerController.Instance.inputMap.Player.Enable();
                break;
            
            case "Show":
                planningUI.enabled = true;
                
                UIManager.Instance.ToggleInteractText(false, "");
                
                PlayerController.Instance.inputMap.UI.Enable();
                PlayerController.Instance.inputMap.Player.Disable();
                break;
            
            case "Details":
                detailsScreenAnim.Play("ShowDetails");
                detailsScreenAnim.SetBool("isHidden", false);
                
                //UIMenuStack.Push(detailsMenu);
                break;
            
            case "Loadout":
                detailsScreenAnim.Play("HideDetails");
                detailsScreenAnim.SetBool("isHidden", true);
                
                UIMenuStack.Push(loadoutMenu);
                break;
            
            case "Progression":
                planningUI.enabled = true;
                
                UIMenuStack.Push(progressionMenu);
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

    private void OnSubmit(InputAction.CallbackContext context) { }
    
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
