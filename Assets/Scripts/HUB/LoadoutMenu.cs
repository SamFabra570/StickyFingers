using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutMenu : MonoBehaviour, IUIMenu
{
    public EventSystem eventSystem;

    [Header("Slots")] 
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    
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

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject != currentSelected &&  eventSystem.currentSelectedGameObject != null) 
            ValidateNavigation();
    }

    public void OnShowMenu()
    {
        UpdateAvailableSlots();
        
        state =  State.AbilitySlotSelect;
        selectedSlot = null;
        
        
        if (slot1.GetComponent<Button>().interactable) 
            eventSystem.SetSelectedGameObject(slot1);
        else
        {
            eventSystem.SetSelectedGameObject(passiveSlot);
        }
    }

    public void OnHideMenu()
    {
        //Optional stuff on closing menu
    }

    public void OnSubmit()
    {
        if (state == State.AbilitySlotSelect)
        {
            SelectAbilitySlot();
        }
        else if (state == State.AbilitySelect)
        {
            SelectAbility();
        }
        else if (state == State.PassiveSlotSelect)
        {
            SelectPassiveSlot();
        }
        else if (state == State.PassiveSelect)
        {
            SelectPassive();
        }
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
    
    //Called when clicking a slot
    public void SelectAbilitySlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;
        
        if (firstAbility != null && firstAbility.interactable) 
            eventSystem.SetSelectedGameObject(firstAbility.gameObject);
        else
        {
            MoveToNextSlot();
        }
        
        if (eventSystem.currentSelectedGameObject == firstAbility.gameObject) 
            state = State.AbilitySelect;
    }
    
    public void SelectAbility()
    {
        GameObject selectedAbility = eventSystem.currentSelectedGameObject;

        DraggableItem slot = selectedAbility.GetComponentInChildren<DraggableItem>();
        
        if (slot == null || selectedSlot == null)
            return;

        if (selectedSlot == slot1)
        {
            AbilityManager.Instance.EquipAbility(0, slot.ability);
        }
        
        else if (selectedSlot == slot2)
        {
            AbilityManager.Instance.EquipAbility(1, slot.ability);
        }
        
        else if (selectedSlot == slot3)
        {
            AbilityManager.Instance.EquipAbility(2, slot.ability);
        }
        
        slot.transform.SetParent(selectedSlot.transform);

        MoveToNextSlot();
    }
    
    private void MoveToNextSlot()
    {
        if (selectedSlot == slot1 && slot2.GetComponent<Button>().interactable)
            eventSystem.SetSelectedGameObject(slot2);
        
        else if (selectedSlot == slot2 && slot3.GetComponent<Button>().interactable)
            eventSystem.SetSelectedGameObject(slot3);
        
        else
        {
            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(passiveSlot);
            return;
        }

        state = State.AbilitySlotSelect;
        selectedSlot = null;
    }
    
    private void ValidateNavigation()
    {
        var current = eventSystem.currentSelectedGameObject;
        
        Debug.Log(current + "loadoutMenu");

        if (current != null)
        {
            var button = current.GetComponent<Button>();

            if (button != null && !button.interactable)
            {
                eventSystem.SetSelectedGameObject(currentSelected);
            }
            else 
                currentSelected =  button.gameObject;
        }
    }
    
    public void SelectPassiveSlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;
        
        state = State.PassiveSelect;
        
        eventSystem.SetSelectedGameObject(firstPassive);
    }
    
    public void SelectPassive()
    {
        GameObject selected = eventSystem.currentSelectedGameObject;

        DraggableItem item = selected.GetComponentInChildren<DraggableItem>();

        if (item == null)
            return;

        PassiveAbilityController playerPassives = GameManager.Instance.PlayerPassives;
        playerPassives.EquipPassive(item);

        state = State.Ready;

        eventSystem.SetSelectedGameObject(readyButton);
    }
    
    public void OnCancel()
    {
        Debug.Log("LoadoutMenu Cancel");
        
        if (state == State.AbilitySlotSelect)
        {
            HUB_UIManager.Instance.TogglePlanningUI("Close");
            return;
        }

        if (state == State.AbilitySelect)
        {
            state = State.AbilitySlotSelect;
            eventSystem.SetSelectedGameObject(selectedSlot);
            return;
        }

        if (state == State.PassiveSlotSelect)
        {
            state = State.AbilitySlotSelect;
            
            if (slot3.GetComponent<Button>().interactable) 
                eventSystem.SetSelectedGameObject(slot3);
            else if (slot2.GetComponent<Button>().interactable)
                eventSystem.SetSelectedGameObject(slot2);
            else if (slot1.GetComponent<Button>().interactable)
                eventSystem.SetSelectedGameObject(slot1);
            else
            {
                HUB_UIManager.Instance.TogglePlanningUI("Close");
            }
            
            return;
        }

        if (state == State.PassiveSelect)
        {
            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(passiveSlot);
            return;
        }

        if (state == State.Ready)
        {
            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(passiveSlot);
        }
    }

    public void OnNavigate(Vector2 direction) { }
}
