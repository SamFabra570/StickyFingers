using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutMenu : MonoBehaviour, IUIMenu
{
    public EventSystem eventSystem;
    
    [Header ("Loadout UI Refs")]
    public Canvas HUBCanvas;
    
    public Animator detailsScreenAnim;
    public bool isAnimDone = true;
    
    public GameObject switchScreenButtonText;
    
    [Header("Debt UI")]
    public Slider debtPaidFill;
    public TextMeshProUGUI debtPaidText;
    public TextMeshProUGUI totalDebtText;

    [Header("Ability Setup Refs")] 
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;

    private int slotNum;

    public GameObject passiveSlot;

    public Button firstAbility;
    public GameObject firstPassive;

    public GameObject readyButton;

    private GameObject selectedSlot;
    private GameObject currentSelected;

    private enum State
    {
        AbilitySlotSelect,
        AbilitySelect,
        PassiveSlotSelect,
        PassiveSelect,
        Ready
    }

    private State state;

    private void Awake()
    {
        switchScreenButtonText.SetActive(false);
        HUBCanvas.enabled = false;
    }

    private void Update()
    {
        if (ReferenceEquals(UIMenuStack.Current, this))
        {
            if (eventSystem.currentSelectedGameObject != currentSelected &&
                eventSystem.currentSelectedGameObject != null)
                ValidateNavigation();
        }
    }

    public void OnShowMenu()
    {
        UpdateAbilityLockState();
        UpdateAvailableSlots();
        UpdateDebtInfo();

        HUBCanvas.enabled = true;
        
        switchScreenButtonText.SetActive(true);

        state = State.AbilitySlotSelect;
        selectedSlot = null;

        if (slot1.GetComponent<Button>().interactable)
            eventSystem.SetSelectedGameObject(slot1);
        else
        {
            selectedSlot = passiveSlot;
            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(passiveSlot);
        }
    }

    public void OnHideMenu()
    {
        TooltipUI.Instance.StopTooltip();
        
        switchScreenButtonText.SetActive(true);
        HUBCanvas.enabled = false;
    }

    public void UpdateDebtInfo()
    {
        debtPaidFill.value = GameManager.Instance.GetDebtPaidPercent();
        debtPaidText.text = ("" + (GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt));
        totalDebtText.text = ("" + GameManager.Instance.maxDebt);
    }
    
    private void UpdateAbilityLockState()
    {
        var abilityUIs = ProgressionMenu.Instance.GetComponentsInChildren<AbilityUnlock>(true);

        foreach (var ui in abilityUIs)
        {
            ui.UpdateState();
        }
    }

    // private GameObject FindNextInteractable(GameObject current)
    // {
    //     var selectable = current.GetComponent<Selectable>();
    //
    //     if (selectable != null)
    //     {
    //         var next = selectable.FindSelectableOnRight();
    //
    //         if (next != null && next.interactable)
    //         {
    //             if (next.gameObject.activeSelf)
    //                 return next.gameObject;
    //                 
    //             return readyButton;
    //         }
    //     }
    //
    //     return readyButton;
    // }

    private void MoveToNextSlot()
    {
        if (selectedSlot == slot1 && slot2.GetComponent<Button>().interactable)
        {
            selectedSlot = slot2;
            slot1.GetComponent<Button>().interactable = false;
        }

        else if (selectedSlot == slot2 && slot3.GetComponent<Button>().interactable)
        {
            selectedSlot = slot3;
            slot2.GetComponent<Button>().interactable = false;
        }

        else if (selectedSlot == slot3)
        {
            selectedSlot = passiveSlot;
            slot3.GetComponent<Button>().interactable = false;

            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(selectedSlot);
            return;
        }
        else
        {
            selectedSlot.GetComponent<Button>().interactable = false;
            selectedSlot = passiveSlot;

            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(selectedSlot);
            return;
        }

        state = State.AbilitySlotSelect;
        eventSystem.SetSelectedGameObject(selectedSlot);
    }

    //Called when clicking a slot
    public void SelectAbilitySlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;

        //Start ability selection
        if (firstAbility != null && firstAbility.interactable)
        {
            state = State.AbilitySelect;
            eventSystem.SetSelectedGameObject(firstAbility.gameObject);
        }
        //If no unlocked abilities left, move on
        else
        {
            MoveToNextSlot();
        }
    }

    public void SelectAbility()
    {
        GameObject selectedAbility = eventSystem.currentSelectedGameObject;

        SetAbility(selectedAbility);

        MoveToNextSlot();
    }

    public void SelectPassiveSlot()
    {
        if (selectedSlot != passiveSlot)
            selectedSlot = eventSystem.currentSelectedGameObject;

        state = State.PassiveSelect;

        eventSystem.SetSelectedGameObject(firstPassive);
    }

    public void SelectPassive()
    {
        GameObject selected = eventSystem.currentSelectedGameObject;

        SetAbility(selected);

        state = State.Ready;

        eventSystem.SetSelectedGameObject(readyButton);
    }

    private void SetAbility(GameObject selected)
    {
        if (selected == null)
            return;

        //If abilityslot already has item, and it's not a placeholder, get outta here
        if (selectedSlot.transform.childCount != 0)
        {
            if (selectedSlot.transform.GetChild(0).CompareTag("Placeholder"))
                Destroy(selectedSlot.transform.GetChild(0).gameObject);
            else
            {
                Debug.Log("Ability already selected!");
                return;
            }
        }

        DraggableItem ability = selected.GetComponentInChildren<DraggableItem>();

        if (ability == null || selectedSlot == null)
        {
            Debug.Log("smth is null lolll");
            return;
        }


        //If ability
        if (ability.abilityType == AbilityType.Ability)
        {
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
        }

        //If passive
        else if (ability.abilityType == AbilityType.Passive)
        {
            PassiveAbilityController playerPassives = GameManager.Instance.PlayerPassives;
            playerPassives.EquipPassive(ability);
        }

        ability.transform.SetParent(selectedSlot.transform);
    }

    private void DeselectAbility(GameObject selected)
    {
        DraggableItem ability = selected.GetComponentInChildren<DraggableItem>();

        if (ability.abilityType == AbilityType.Ability)
        {
            if (selectedSlot == slot1)
                slotNum = 0;
            else if (selectedSlot == slot2)
                slotNum = 1;
            else if (selectedSlot == slot3)
                slotNum = 2;

            AbilityManager.Instance.DequipAbility(slotNum);
        }
        else if (ability.abilityType == AbilityType.Passive)
        {
            PassiveAbilityController playerPassives = GameManager.Instance.PlayerPassives;
            playerPassives.DequipPassive(ability);
        }
        
        ability.transform.SetParent(ability.originalParent);
    }

    private void UpdateAvailableSlots()
    {
        if (ProgressionManager.Instance.unlockedAbilities.Count == 0)
        {
            slot1.GetComponent<Button>().interactable = false;
            slot2.GetComponent<Button>().interactable = false;
            slot3.GetComponent<Button>().interactable = false;
        }
        else if (ProgressionManager.Instance.unlockedAbilities.Count == 1)
        {
            slot1.GetComponent<Button>().interactable = true;
            slot2.GetComponent<Button>().interactable = false;
            slot3.GetComponent<Button>().interactable = false;
        }
        else if (ProgressionManager.Instance.unlockedAbilities.Count == 2)
        {
            slot1.GetComponent<Button>().interactable = true;
            slot2.GetComponent<Button>().interactable = true;
            slot3.GetComponent<Button>().interactable = false;
        }
        else
        {
            slot1.GetComponent<Button>().interactable = true;
            slot2.GetComponent<Button>().interactable = true;
            slot3.GetComponent<Button>().interactable = true;
        }
    }
    
    private void ValidateNavigation()
    {
        var current = eventSystem.currentSelectedGameObject;

        if (current != null)
        {
            var button = current.GetComponent<Button>();

            if (button != null && !button.interactable)
            {
                eventSystem.SetSelectedGameObject(currentSelected);
            }
            else
                currentSelected = button.gameObject;
        }
    }
    
    public void OnCancel()
    {
        switch (state)
        {
            case (State.AbilitySlotSelect): //Cancel while selecting ability slot
                //If slot is not empty, and child of slot is an ability, deselect ability
                if (selectedSlot.transform.childCount != 0)
                {
                    if (selectedSlot.transform.GetChild(0).CompareTag("Ability"))
                    {
                        DeselectAbility(selectedSlot.transform.GetChild(0).gameObject);
                        TooltipUI.Instance.StopTooltip();
                        return;
                    }
                }

                if (selectedSlot == slot3)
                {
                    slot2.GetComponent<Button>().interactable = true;
                    selectedSlot = slot2;
                }

                else if (selectedSlot == slot2)
                {
                    slot1.GetComponent<Button>().interactable = true;
                    selectedSlot = slot1;
                }
                else if (selectedSlot == slot1)
                {
                    UIManager.Instance.HideMenu();
                    return;
                }

                eventSystem.SetSelectedGameObject(selectedSlot);
                return;
            
            
            case (State.AbilitySelect): //Cancel while selecting ability
                if (!selectedSlot.GetComponent<Button>().interactable)
                    selectedSlot.GetComponent<Button>().interactable = true;

                eventSystem.SetSelectedGameObject(selectedSlot);

                state = State.AbilitySlotSelect;
                return;
            
            
            case  (State.PassiveSlotSelect): //Cancel while selecting passive slot
                //If slot is not empty, and child of slot is an passive, deselect ability
                if (selectedSlot.transform.childCount != 0)
                {
                    if (selectedSlot.transform.GetChild(0).CompareTag("Passive"))
                    {
                        DeselectAbility(selectedSlot.transform.GetChild(0).gameObject);
                        TooltipUI.Instance.StopTooltip();
                        return;
                    }
                }
            
                if (!slot3.GetComponent<Button>().interactable)
                {
                    if (slot3.transform.childCount == 0 || !slot3.transform.GetChild(0).CompareTag("Placeholder"))
                    {
                        slot3.GetComponent<Button>().interactable = true;
                        selectedSlot = slot3;
                    }

                    else if (!slot2.GetComponent<Button>().interactable)
                    {
                        if (slot2.transform.childCount == 0 || !slot2.transform.GetChild(0).CompareTag("Placeholder"))
                        {
                            slot2.GetComponent<Button>().interactable = true;
                            selectedSlot = slot2;
                        }

                        else if (!slot1.GetComponent<Button>().interactable)
                        {
                            if (slot1.transform.childCount == 0 || !slot1.transform.GetChild(0).CompareTag("Placeholder"))
                            {
                                slot1.GetComponent<Button>().interactable = true;
                                selectedSlot = slot1;
                            }
                        }
                    }
                }

                if (selectedSlot != passiveSlot)
                {
                    state = State.AbilitySlotSelect;
                    eventSystem.SetSelectedGameObject(selectedSlot);
                }
                else
                {
                    UIManager.Instance.HideMenu();
                }

                break;
            
            
            case (State.PassiveSelect): //Cancel while selecting passive
                selectedSlot = passiveSlot;

                state = State.PassiveSlotSelect;

                eventSystem.SetSelectedGameObject(selectedSlot);
                return;
            
            case (State.Ready): //Cancel while ready
                selectedSlot = passiveSlot;

                state = State.PassiveSlotSelect;

                eventSystem.SetSelectedGameObject(selectedSlot);
                break;
        }
    }

    public void OnButtonNorth()
    {
        ToggleDetailsScreen();
    }

    private void ToggleDetailsScreen()
    {
        if (isAnimDone)
        {
            if (detailsScreenAnim.GetBool("isHidden"))
            {
                UpdateDebtInfo();
                TooltipUI.Instance.StopTooltip();
                
                detailsScreenAnim.Play("ShowDetails");
                detailsScreenAnim.SetBool("isHidden", false);
            }
            else
            {
                detailsScreenAnim.Play("HideDetails");
                detailsScreenAnim.SetBool("isHidden", true);
            }
                
            isAnimDone = false;
        }
    }

    
}