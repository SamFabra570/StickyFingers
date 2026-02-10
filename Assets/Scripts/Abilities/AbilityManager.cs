using System;
using System.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    
    public AbilitySlot[] abilities = new AbilitySlot[3];

    private AbilityState state;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        ResetAbilityCooldowns();
    }

    private void Update()
    {
        foreach (var slot in abilities)
        {
            if (slot?.ability != null)
            {
                slot.UpdateCooldown(Time.deltaTime);
                slot.UpdateDuration(Time.deltaTime);
            }
        }
    }

    public void ActivateAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilities.Length)
        {
            Debug.LogWarning("ActivateAbility: Invalid Slot Index");
            return;
        }
        
        AbilitySlot slot = abilities[slotIndex];
        
        if (slot.ability == null)
            return;

        //Check if ability is ready, if not, exit method
        if (!slot.IsReady())
        {
            Debug.Log(slot.ability.name + " not ready!");
            return;
        }
        
        slot.StartDuration();
        slot.ability.Activate(gameObject);
    }

    public void DeactivateAbility(AbilitySlot slot)
    {
        slot.ability.Deactivate(gameObject);
             
        slot.isActive = false;
        slot.cooldownRemaining = slot.ability.cooldown;
        slot.state = AbilityState.Cooldown;
    }

    public void EquipAbility(int slotIndex, AbilitySlot ability)
    {
        if (slotIndex < 0 || slotIndex >= abilities.Length)
            Debug.LogWarning("EquipAbility: Invalid Slot Index");
        
        abilities[slotIndex] = ability;
    }

    public void DequipAbility(int slotIndex)
    {
        abilities[slotIndex] = null;
    }

    public AbilitySlot GetAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilities.Length)
        {
            Debug.LogWarning("GetAbility: Invalid Slot Index");
            return null;
        }
        
        return abilities[slotIndex];
    }

    public void ResetAbilityCooldowns()
    {
        //Reset all equipped abilities
        foreach (var slot in abilities)
        {
            if (slot != null)
            {
                slot.cooldownRemaining = 0f;
                slot.state = AbilityState.Ready;
            }
                
        }
    }
}
