using UnityEngine;
using UnityEngine.EventSystems;

public class LoadoutMenu : MonoBehaviour, IUIMenu
{
    public EventSystem eventSystem;

    [Header("Slots")] 
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    
    public GameObject passiveSlot;
    
    public GameObject firstAbility;
    public GameObject firstPassive;
    
    public GameObject readyButton;

    private GameObject selectedSlot;

    private enum State
    {
        AbilitySlotSelect,
        AbilitySelect,
        PassiveSlotSelect,
        PassiveSelect,
        Ready
    }

    private State state;

    public void OnShowMenu()
    {
        state =  State.AbilitySlotSelect;
        selectedSlot = null;
        eventSystem.SetSelectedGameObject(slot1);
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
    
    //Called when clicking a slot
    public void SelectAbilitySlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;

        state = State.AbilitySelect;
        
        eventSystem.SetSelectedGameObject(firstAbility);
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
        if (selectedSlot == slot1)
            eventSystem.SetSelectedGameObject(slot2);
        
        else if (selectedSlot == slot2)
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
    
    public void SelectPassiveSlot()
    {
        selectedSlot = eventSystem.currentSelectedGameObject;
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
            eventSystem.SetSelectedGameObject(slot3);
            return;
        }

        if (state == State.PassiveSelect)
        {
            state = State.PassiveSlotSelect;
            eventSystem.SetSelectedGameObject(passiveSlot);
            return;
        }
    }

    public void OnNavigate(Vector2 direction) { }
}
