using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAbilitySlot : MonoBehaviour, IDropHandler
{
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

    private void SetAbilitySlot(AbilitySlot ability)
    {
        if (gameObject.CompareTag("Slot1"))
        {
            AbilityManager.Instance.EquipAbility(0, ability);
            Debug.Log("Ability 1 set");
        }
            
        else if (gameObject.CompareTag("Slot2"))
        {
            AbilityManager.Instance.EquipAbility(1, ability);
            Debug.Log("Ability 2 set");
        }
            
        else if (gameObject.CompareTag("Slot3"))
        {
            AbilityManager.Instance.EquipAbility(2, ability);
            Debug.Log("Ability 3 set: " + ability);
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
    }
}
