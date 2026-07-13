using System;
using System.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    
    public AbilitySlot[] abilities = new AbilitySlot[3];

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
        foreach (AbilitySlot slot in abilities)
        {
            if (slot == null) continue;

            if (slot.ability == null) continue;
            
            slot.UpdateCooldown(Time.deltaTime);
            slot.UpdateDuration(Time.deltaTime);
        }
    }

    public void ActivateAbility(int slotIndex)
    {
        if (IsAbilityActive())
        {
            Debug.Log("Ability already active");
            return;
        }
        
        if (slotIndex < 0 || slotIndex >= abilities.Length)
        {
            Debug.LogWarning("ActivateAbility: Invalid Slot Index");
            return;
        }
        
        AbilitySlot slot = abilities[slotIndex];

        if (slot.ability == null)
        {
            Debug.Log("ability not equipped");
            return;
        }

        //Check if ability is ready, if not, exit method
        if (!slot.IsReady)
        {
            Debug.Log(slot.ability.name + " not ready!");
            return;
        }
        
        slot.ability.Activate(gameObject);
        slot.BeginActive();
    }

    public void DeactivateAbility(AbilitySlot slot)
    {
        slot.ability.Deactivate(gameObject);

        slot.BeginCooldown();
    }
    
    public bool IsAbilityActive()
    {
        foreach (AbilitySlot slot in abilities)
        {
            if (slot != null && slot.IsActive)
                return true;
        }

        return false;
    }

    public void InterruptAllAbilities()
    {
        foreach (AbilitySlot slot in abilities)
        {
            if (slot == null || slot.ability == null)
                continue;

            slot.ability.Deactivate(gameObject);

            slot.Disable();
        }
    }

    public void EquipAbility(int slotIndex, AbilitySlot ability)
    {
        if (slotIndex < 0 || slotIndex >= abilities.Length)
        {
            Debug.LogWarning("EquipAbility: Invalid Slot Index");
            return;
        }
        
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
        foreach (AbilitySlot slot in abilities)
        {
            if (slot != null)
                slot.SetReady();
        }
    }
}
