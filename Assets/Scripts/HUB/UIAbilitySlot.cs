using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAbilitySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public Button slotButton;

    public bool isContainer = true;
    public GameObject abilityLockOverlay;
    
    public enum SlotState
    {
        Locked,
        Empty,
        Full
    }
    
    public SlotState slotState = SlotState.Locked;

    private void Start()
    {
        slotButton = GetComponent<Button>();
        
        CheckIfUnlocked();
    }

    private void Update()
    {
        if (ProgressionManager.Instance.isDirty)
        {
            CheckIfUnlocked();
            
            ProgressionManager.Instance.isDirty = false;
        }
        
        UpdateSlotBasedOnState();
    }

    private void CheckIfUnlocked()
    {
        if (isContainer)
        {
            Ability ability = GetComponentInChildren<DraggableItem>().ability.ability;

            if (ProgressionManager.Instance.IsUnlocked(ability))
            {
                UpdateSlotState(SlotState.Full);
            }
            else
            {
                UpdateSlotState(SlotState.Locked);
            }
        }
        
    }

    public void UpdateSlotState(SlotState state)
    {
        slotState = state;
        
        Debug.Log("New state: " + slotState);
        
        UpdateSlotBasedOnState();
    }

    public void UpdateSlotBasedOnState()
    {
        {
            switch (slotState)
            {
                case SlotState.Locked: 
                    abilityLockOverlay.SetActive(true);
                    slotButton.interactable = false;
                    break;
                case SlotState.Empty:
                    if (!isContainer)
                    {
                        ResetAbilitySlot();
                        transform.GetChild(0).gameObject.SetActive(true);
                    }
                    
                    abilityLockOverlay.SetActive(false);
                    slotButton.interactable = true;
                    break;
                case SlotState.Full:
                    abilityLockOverlay.SetActive(false);
                    slotButton.interactable = true;
                    break;
            }
        }
    }

    private void SetAbilitySlot(AbilitySlot slot)
    {
        if (gameObject.CompareTag("Slot1"))
        {
            AbilityManager.Instance.EquipAbility(0, slot);
            Debug.Log("Ability 1 set" + slot.ability);
        }
            
        else if (gameObject.CompareTag("Slot2"))
        {
            AbilityManager.Instance.EquipAbility(1, slot);
            Debug.Log("Ability 2 set" + slot.ability);
        }
            
        else if (gameObject.CompareTag("Slot3"))
        {
            AbilityManager.Instance.EquipAbility(2, slot);
            Debug.Log("Ability 3 set: " + slot.ability);
        }
        
        UpdateSlotState(SlotState.Full);
    }

    private void ResetAbilitySlot()
    {
        if (gameObject.CompareTag("Slot1"))
            AbilityManager.Instance.DequipAbility(0);
        
        else if (gameObject.CompareTag("Slot2"))
            AbilityManager.Instance.DequipAbility(1);
        
        else if(gameObject.CompareTag("Slot3"))
            AbilityManager.Instance.DequipAbility(2);
    }
    
    private void OnTransformChildrenChanged()
    {
        if (!isContainer)
        {
            if (transform.childCount <= 1)
                UpdateSlotState(SlotState.Empty);
            else
                UpdateSlotState(SlotState.Full);
        }
        else if (isContainer)
        {
            if (transform.childCount == 0)
                UpdateSlotState(SlotState.Empty);
            else
                UpdateSlotState(SlotState.Full);
        }
        
        //Debug.Log(gameObject + (", children: ") + transform.childCount + (", slotState: " + slotState));
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (slotState != SlotState.Locked)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            
            if (!isContainer)
            {
                if (transform.GetChild(0).CompareTag("Placeholder"))
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
                
                SetAbilitySlot(draggableItem.ability);
            }
            
            draggableItem.parentAfterDrag = transform;
            
            UpdateSlotState(SlotState.Full);
        }
    }
}
