using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAbilitySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject equippedAbility;
    
    private void Update()
    {
        if (transform.childCount == 0)
            ResetAbilitySlot();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
            
            SetAbilitySlot(draggableItem.ability);
        }
    }

    private void SetAbilitySlot(AbilitySlot slot)
    {
        if (gameObject.CompareTag("Slot1"))
        {
            AbilityManager.Instance.EquipAbility(0, slot);
            //equippedAbility = abilityObject.gameObject;
            Debug.Log("Ability 1 set" + slot.ability);
        }
            
        else if (gameObject.CompareTag("Slot2"))
        {
            AbilityManager.Instance.EquipAbility(1, slot);
            //equippedAbility = abilityObject.gameObject;
            Debug.Log("Ability 2 set" + slot.ability);
        }
            
        else if (gameObject.CompareTag("Slot3"))
        {
            AbilityManager.Instance.EquipAbility(2, slot);
            //equippedAbility = abilityObject.gameObject;
            Debug.Log("Ability 3 set: " + slot.ability);
        }
    }

    private void ResetAbilitySlot()
    {
        if (gameObject.CompareTag("Slot1"))
            AbilityManager.Instance.DequipAbility(0);
        else if (gameObject.CompareTag("Slot2"))
            AbilityManager.Instance.DequipAbility(1);
        else if(gameObject.CompareTag("Slot3"))
            AbilityManager.Instance.DequipAbility(2);
        
        //equippedAbility = null;
    }

    private void SetActiveAbilities()
    {
        if (gameObject.CompareTag("Slot1"))
            Debug.Log("Ability 1 active");
        else if (gameObject.CompareTag("Slot2"))
            AbilityManager.Instance.DequipAbility(1);
        else if(gameObject.CompareTag("Slot3"))
            AbilityManager.Instance.DequipAbility(2);
    }
}
