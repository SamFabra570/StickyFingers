using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Range (0,2)]
    public int slotIndex;

    private bool isActive;
    
    public Image cooldownFill;
    public Image durationFill;

    // Update is called once per frame
    void Update()
    {
        AbilitySlot slot = AbilityManager.Instance.GetAbility(slotIndex);

        if (slot.state == AbilityState.Ready)
        {
            cooldownFill.fillAmount = 0;
            durationFill.fillAmount = 0;
        }

        if (slot.state ==  AbilityState.Cooldown)
        {
            durationFill.fillAmount = 0;
            
            cooldownFill.fillAmount = 1;
            
            float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
            cooldownFill.fillAmount = normalizedCooldown;
        }
        
        if (slot.state ==  AbilityState.Active |  slot.state ==  AbilityState.Ending)
        {
            cooldownFill.fillAmount = 0;
            
            float normalizedDuration = slot.durationRemaining / slot.ability.duration;
            durationFill.fillAmount = 1 - normalizedDuration;
        }
    }
}
