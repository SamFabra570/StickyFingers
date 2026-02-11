using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Range (0,2)]
    public int slotIndex;

    public Image abilityFrame;
    public Image abilityIcon;
    public Image cooldownFill;
    public Image durationFill;

    // Update is called once per frame
    void Update()
    {
        AbilityManager manager = AbilityManager.Instance;
        AbilitySlot slot = manager.GetAbility(slotIndex);
        
        SetFrameColour(slot);

        if (slot == null || slot.ability == null)
        {
            abilityIcon.gameObject.SetActive(false);
            return;
        }
        
        SetIcon(slot);
        
        bool anotherAbilityActive = manager.activeSlot != null 
                                    && manager.activeSlot.ability != null 
                                        && manager.activeSlot != slot;

        //Ability cooldown
        if (slot.state ==  AbilityState.Cooldown)
        {
            durationFill.fillAmount = 0;
            
            cooldownFill.fillAmount = 1;
            
            float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
            cooldownFill.fillAmount = normalizedCooldown;
            return;
        }
        
        //Ability active
        if (IsThisActiveAbility(slot, manager) && 
            (slot.state ==  AbilityState.Active ||  slot.state ==  AbilityState.Ending))
        {
            cooldownFill.fillAmount = 0;
            
            float normalizedDuration = slot.durationRemaining / slot.ability.duration;
            durationFill.fillAmount = 1 - normalizedDuration;
            return;
        }
        
        if (anotherAbilityActive)
        {
            if (!IsThisActiveAbility(slot, manager)) 
                durationFill.fillAmount = 1f;
            return;
        }
        
        //Ability Ready
        cooldownFill.fillAmount = 0;
        durationFill.fillAmount = 0;
    }

    private void SetFrameColour(AbilitySlot slot)
    {
        if (slot.ability == null)
        {
            abilityFrame.color = Color.gray3;
            return;
        }
        
        if (abilityFrame.color != slot.ability.abilityColour.color) 
            abilityFrame.color = slot.ability.abilityColour.color;
    }

    private bool IsThisActiveAbility(AbilitySlot slot, AbilityManager manager)
    {
        bool isActiveAbility = slot.ability == manager.activeSlot.ability;
        
        return isActiveAbility;
    }

    private void SetIcon(AbilitySlot slot)
    {
        abilityIcon.sprite = slot.ability.icon;
    }
    
}
