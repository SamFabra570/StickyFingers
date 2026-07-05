using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutMenu : MonoBehaviour, IUIMenu
{
    public static LoadoutMenu Instance;
    
    public EventSystem eventSystem;
    
    [Header ("Loadout UI Refs")]
    public Canvas loadoutCanvas;
    
    [Header("Debt UI")]
    public Slider debtPaidFill;
    public TextMeshProUGUI debtPaidText;
    public TextMeshProUGUI totalDebtText;

    [Header("Ability Setup Refs")] 
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;

    private int slotNum;

    public Button firstAbility;
    private UIAbilitySlot slot;
    
    public GameObject firstPassive;

    public GameObject readyButton;
    public GameObject backButton;

    private GameObject selectedSlot;
    private GameObject lastSelected;

    private GameObject selectedAbilityUI;
    private GameObject selectedPassive;

    [SerializeField] private GameObject selectionImage;
    
    [SerializeField] private GameObject abilityUnlockButtons;

    private enum State
    {
        AbilitySlotSelect,
        AbilitySelect,
        PassiveSelect,
        Ready
    }

    private State state;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        loadoutCanvas.enabled = false;
    }

    private void Update()
    {
        if (ReferenceEquals(UIMenuStack.Current, this))
        {
            if (eventSystem.currentSelectedGameObject != lastSelected 
                && eventSystem.currentSelectedGameObject != null)
                ValidateNavigation();
        }
    }

    public void OnShowMenu()
    {
        UpdateMissionButtonState(false);
        UpdateAvailableSlots();
        UpdateDebtInfo();


        ProgressionMenu.Instance.backButton.interactable = false;
        loadoutCanvas.enabled = true;

        state = State.AbilitySlotSelect;
        selectedSlot = slot1;

        if (slot1.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
        {
            SetSelection(selectedSlot);   
        }
        else 
            MoveToNextSlot();
    }

    public void OnHideMenu()
    {
        UpdateMissionButtonState(true);
        
        TooltipUI.Instance.StopTooltip();
        eventSystem.SetSelectedGameObject(null);
        
        ProgressionMenu.Instance.backButton.interactable = true;
        loadoutCanvas.enabled = false;
        
        UIManager.Instance.ToggleInteractText(true, "");
    }

    public void UpdateDebtInfo()
    {
        debtPaidFill.value = GameManager.Instance.GetDebtPaidPercent();
        debtPaidText.text = ("" + (GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt));
        totalDebtText.text = ("" + GameManager.Instance.maxDebt);
    }
    
    private void UpdateMissionButtonState(bool active)
    {
        var missionButtons = abilityUnlockButtons.GetComponentsInChildren<Button>(true);
        
        foreach (var button in missionButtons)
        {
            if (!active) 
                button.interactable = false;
            else 
                button.interactable = true;
        }
    }

    private void MoveToNextSlot()
    {
        if (slot2.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
        {
            selectedSlot = slot2;
        }

        else if (slot3.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
        {
            selectedSlot = slot3;
        }

        else
        {
            if (selectedPassive != null)
                SetSelection(selectedPassive);
            else
                SetSelection(firstPassive);

            state = State.PassiveSelect;
            return;
        }

        state = State.AbilitySlotSelect;
        SetSelection(selectedSlot);
    }

    //Called when clicking a slot
    public void SelectAbilitySlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;

        //Start ability selection
        if (firstAbility.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
        {
            state = State.AbilitySelect;
            SetSelection(firstAbility.gameObject);
        }
        //If no unlocked abilities left, move on
        else
        {
            MoveToNextSlot();
        }
    }

    public void SelectAbility()
    {
        DraggableItem selectedAbility = eventSystem.currentSelectedGameObject.GetComponentInChildren<DraggableItem>();

        SetAbility(selectedAbility);

        MoveToNextSlot();
    }

    public void SelectPassive(Passive passive)
    {
        if (selectedPassive != null)
        {
            //If selecting already equipped passive, deselect only
            if (GameManager.Instance.PlayerPassives.Has(passive.passiveID))
            {
                DeselectAbility(selectedPassive);
                return;
            }
            
            DeselectAbility(selectedPassive);
        }
        
        GameManager.Instance.PlayerPassives.EquipPassive(passive);
        
        selectedPassive = eventSystem.currentSelectedGameObject;
        selectedPassive.GetComponent<Image>().color = Color.gray2;
        
        lastSelected = selectedPassive;
        
        state = State.Ready;

        SetSelection(readyButton);
    }

    private void SetAbility(DraggableItem ability)
    {
        Debug.Log("Selected slot: " + selectedSlot);
        
        if (ability == null)
        {
            ability = selectedSlot.GetComponentInChildren<DraggableItem>();
        }
        
        UIAbilitySlot selectedSlotState = selectedSlot.GetComponent<UIAbilitySlot>();
        
        //Debug.Log("Slot: " + selectedSlot + ", State: " + selectedSlotState.slotState);
        
        switch (selectedSlotState.slotState)
        {
            case UIAbilitySlot.SlotState.Locked:
                return;
            
            case UIAbilitySlot.SlotState.Empty:
                if (slot.slotState == UIAbilitySlot.SlotState.Empty)
                {
                    //selectedSlot.transform.GetChild(0).gameObject.SetActive(true);
                    Debug.Log("No ability in container");
                    return;
                }
                
                if (selectedSlot.transform.childCount != 0)
                     selectedSlot.transform.GetChild(0).gameObject.SetActive(false);
                break;
            
            case UIAbilitySlot.SlotState.Full:
                if (slot.slotState == UIAbilitySlot.SlotState.Empty)
                {
                    DeselectAbility(selectedSlot.transform.GetChild(1).gameObject);
                    Debug.Log("No ability in container, resetting slot");
                    return;
                }
                
                DeselectAbility(selectedSlot.transform.GetChild(1).gameObject);
                selectedSlot.GetComponent<UIAbilitySlot>().transform.GetChild(0).gameObject.SetActive(false);
                break;
        }

        if (selectedSlot == slot1)
        {
            AbilityManager.Instance.EquipAbility(0, ability.ability);
        }

        else if (selectedSlot == slot2)
        {
            AbilityManager.Instance.EquipAbility(1, ability.ability);
        }

        else if (selectedSlot == slot3)
        {
            AbilityManager.Instance.EquipAbility(2, ability.ability);
        }
        else
        {
            Debug.Log("ability not equipped: " + selectedSlot);
        }

        selectedSlotState.UpdateSlotState(UIAbilitySlot.SlotState.Full);
        selectedAbilityUI = lastSelected;
        ability.transform.SetParent(selectedSlot.transform);
    }

    private void DeselectAbility(GameObject selected)
    {
        if (selected.CompareTag("Passive"))
        {
            GameManager.Instance.PlayerPassives.DequipPassive();
            selectedPassive.GetComponent<Image>().color = selectedPassive.GetComponent<PassiveButton>().passive.passiveColour.color;
            selectedPassive = null;
        }
        else if (selected.CompareTag("Ability"))
        {
            DraggableItem ability = selected.GetComponent<DraggableItem>();
            
            if (selectedSlot == slot1)
                slotNum = 0;
            else if (selectedSlot == slot2)
                slotNum = 1;
            else if (selectedSlot == slot3)
                slotNum = 2;

            AbilityManager.Instance.DequipAbility(slotNum);
            ability.transform.SetParent(ability.originalParent);
        }
    }

    private void UpdateAvailableSlots()
    {
        selectionImage.SetActive(true);
        
        if (ProgressionManager.Instance.unlockedAbilities.Count >= 1 
            && slot1.GetComponent<UIAbilitySlot>().slotState == UIAbilitySlot.SlotState.Locked)
        {
            Debug.Log("Unlock slot 1");
            slot1.GetComponent<UIAbilitySlot>().UpdateSlotState(UIAbilitySlot.SlotState.Empty);
            
            if (ProgressionManager.Instance.unlockedAbilities.Count >= 2 
                && slot2.GetComponent<UIAbilitySlot>().slotState == UIAbilitySlot.SlotState.Locked)
            {
                Debug.Log("Unlock slot 2");
                slot2.GetComponent<UIAbilitySlot>().UpdateSlotState(UIAbilitySlot.SlotState.Empty);
                
                if (ProgressionManager.Instance.unlockedAbilities.Count >= 3 
                    && slot3.GetComponent<UIAbilitySlot>().slotState == UIAbilitySlot.SlotState.Locked)
                {
                    slot3.GetComponent<UIAbilitySlot>().UpdateSlotState(UIAbilitySlot.SlotState.Empty);
                }
            }
        }
        else
            selectionImage.SetActive(false);
    }
    
    public void SetSelection(GameObject target)
    {
        if (target == null) return;
        
        if (target.TryGetComponent<Button>(out var button))
            if (button == null)
                return;

        slot = target.GetComponent<UIAbilitySlot>();

        if (slot != null)
        {
            if (slot.slotState == UIAbilitySlot.SlotState.Locked)
                return;
            
            if (!selectionImage.activeSelf)
                selectionImage.SetActive(true);
            
            selectionImage.transform.position =  target.transform.position;
        }
        else
        {
            selectionImage.SetActive(false);
        }
        
        eventSystem.SetSelectedGameObject(target);
        lastSelected = target;
    }
    
    private void ValidateNavigation()
    {
        var current = eventSystem.currentSelectedGameObject;
        if (current == null) return;
        
        Debug.Log(current.gameObject.name);

        slot = current.GetComponent<UIAbilitySlot>();

        //Always track last valid selection
        lastSelected = current;
        
        //Passive
        if (current.CompareTag("Passive"))
        {
            selectionImage.SetActive(false);
            
            state = State.PassiveSelect;
        }
        //Ready button
        else if (current.CompareTag("ReadyButton"))
        {
            selectionImage.SetActive(false);
            
            state = State.Ready;
        }
        //Back button
        else if (current.CompareTag("BackButton"))
        {
            selectionImage.SetActive(false);
        }
        //Slots/Containers
        else if (slot != null)
        {
            if (slot.isContainer)
                state = State.AbilitySelect;
            else
            {
                selectedSlot = current;
                state = State.AbilitySlotSelect;
            }
            
            if (!selectionImage.activeSelf)
                selectionImage.SetActive(true);
            
            selectionImage.transform.position = lastSelected.transform.position;
        }
    }

    public void OnCancel()
    {
        if (lastSelected == backButton)
        {
            OnHideMenu();
            return;
        }
            
        
        SetSelection(backButton);
    }
    
    // public void OnCancel() Fix this if u wanna lol
    // {
    //     // switch (state)
    //     // {
    //     //     case (State.AbilitySlotSelect): //Cancel while selecting ability slot
    //     //         //If slot is not empty, and child of slot is an ability, deselect ability
    //     //         if (selectedSlot.transform.childCount != 0)
    //     //         {
    //     //             if (selectedSlot.transform.GetChild(0).CompareTag("Ability"))
    //     //             {
    //     //                 DeselectAbility(selectedSlot.transform.GetChild(1).gameObject);
    //     //                 TooltipUI.Instance.StopTooltip();
    //     //                 return;
    //     //             }
    //     //         }
    //     //
    //     //         if (selectedSlot == slot3)
    //     //         {
    //     //             slot2.GetComponent<Button>().interactable = true;
    //     //             selectedSlot = slot2;
    //     //         }
    //     //
    //     //         else if (selectedSlot == slot2)
    //     //         {
    //     //             slot1.GetComponent<Button>().interactable = true;
    //     //             selectedSlot = slot1;
    //     //         }
    //     //         else if (selectedSlot == slot1)
    //     //         {
    //     //             UIManager.Instance.HideMenu();
    //     //             return;
    //     //         }
    //     //
    //     //         SetSelection(selectedSlot);
    //     //         return;
    //     //     
    //     //     
    //     //     case (State.AbilitySelect): //Cancel while selecting ability
    //     //         if (!selectedSlot.GetComponent<Button>().interactable)
    //     //             selectedSlot.GetComponent<Button>().interactable = true;
    //     //
    //     //         SetSelection(selectedSlot);
    //     //
    //     //         state = State.AbilitySlotSelect;
    //     //         return;
    //     //     
    //     //     case (State.PassiveSelect): //Cancel while selecting passive
    //     //
    //     //         //If passive was already selected, deselect it
    //     //         if (selectedPassive != null)
    //     //         {
    //     //             DeselectAbility(selectedPassive);
    //     //             //eventSystem.SetSelectedGameObject(selectedPassive);
    //     //             return;
    //     //         }
    //     //         
    //     //         if (slot3.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
    //     //         {
    //     //             if (slot3.transform.childCount == 0 || !slot3.transform.GetChild(0).CompareTag("Placeholder"))
    //     //             {
    //     //                 slot3.GetComponent<Button>().interactable = true;
    //     //                 
    //     //             }
    //     //
    //     //             else if (!slot2.GetComponent<Button>().interactable)
    //     //             {
    //     //                 if (slot2.transform.childCount == 0 || !slot2.transform.GetChild(0).CompareTag("Placeholder"))
    //     //                 {
    //     //                     slot2.GetComponent<Button>().interactable = true;
    //     //                     selectedSlot = slot2;
    //     //                 }
    //     //
    //     //                 else if (!slot1.GetComponent<Button>().interactable)
    //     //                 {
    //     //                     if (slot1.transform.childCount == 0 || !slot1.transform.GetChild(0).CompareTag("Placeholder"))
    //     //                     {
    //     //                         slot1.GetComponent<Button>().interactable = true;
    //     //                         selectedSlot = slot1;
    //     //                     }
    //     //                 }
    //     //             }
    //     //             
    //     //             SetSelection(selectedSlot);
    //     //         }
    //     //
    //     //         if (slot3.GetComponent<UIAbilitySlot>().slotState != UIAbilitySlot.SlotState.Locked)
    //     //         {
    //     //             
    //     //         }
    //     //         
    //     //         
    //     //         else 
    //     //             UIManager.Instance.HideMenu();
    //     //         
    //     //         return;
    //     //     
    //     //     case (State.Ready): //Cancel while ready
    //     //         
    //     //         if (selectedPassive != null) 
    //     //             SetSelection(selectedPassive);
    //     //         else
    //     //             SetSelection(lastSelected);
    //     //         
    //     //         state = State.PassiveSelect;
    //     //         break;
    //     // }
    // }
}