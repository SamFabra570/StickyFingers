using System;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    
    public AbilitySlot[] abilities = new AbilitySlot[3];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Reset all equipped abilities
        foreach (var slot in abilities)
        {
            if (slot != null)
                slot.cooldownRemaining = 0f;
        }
    }

    private void Update()
    {
        foreach (var slot in abilities)
        {
            if (slot?.ability != null)
                slot.UpdateCooldown(Time.deltaTime);
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

        //Check if ability is ready, if not, exit method
        if (!slot.IsReady())
        {
            Debug.Log(slot.ability.name + " on cooldown!");
            return;
        }
            

        if (slot.ability == null)
            return;
        
        //Trigger ability
        slot.ability.Activate(gameObject);
        
        //Reset cooldown
        slot.cooldownRemaining = slot.ability.cooldown;
    }

    public void EquipAbility(int slotIndex, AbilitySlot ability)
    {
        if (slotIndex < 0 || slotIndex >= abilities.Length)
            Debug.LogWarning("EquipAbility: Invalid Slot Index");
        
        abilities[slotIndex] = ability;
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
}
