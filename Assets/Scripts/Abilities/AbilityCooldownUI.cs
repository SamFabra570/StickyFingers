using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Range (0,2)]
    public int slotIndex;

    public Sprite abilityEmptyIcon;
    public Image abilityIcon;
    public Image cooldownFill;
    public Image durationFill;

    // Update is called once per frame
    void Update()
    {
        AbilityManager manager = AbilityManager.Instance;
        AbilitySlot slot = manager.GetAbility(slotIndex);

        if (slot == null || slot.ability == null)
        {
            SetIcon(null);
            return;
        }
        
        SetIcon(slot);
        
        if (slot.IsDisabled)
        {
            abilityIcon.color = Color.gray2;
            return;
        }

        //Ability cooldown
        if (slot.IsOnCooldown)
        {
            durationFill.fillAmount = 0;
            
            cooldownFill.fillAmount = 1;
            
            float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
            cooldownFill.fillAmount = normalizedCooldown;
            return;
        }
        
        //Ability active - duration fill
        if (slot.IsActive)
        {
            cooldownFill.fillAmount = 0;
            
            float normalizedDuration = slot.durationRemaining / slot.ability.duration;
            durationFill.fillAmount = 1 - normalizedDuration;
            return;
        }
        
        if (manager.IsAbilityActive() && !slot.IsActive)
        {
            durationFill.fillAmount = 1f;
            return;
        }

        
        
        //Ability Ready
        cooldownFill.fillAmount = 0;
        durationFill.fillAmount = 0;
    }

    private void SetIcon(AbilitySlot slot)
    {
        if (slot == null)
        {
            abilityIcon.sprite = abilityEmptyIcon;
            abilityIcon.color = Color.gray3;
            return;
        }
        
        abilityIcon.sprite = slot.ability.icon;
    }
    
}
