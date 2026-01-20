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

        if (slot.cooldownRemaining > 0)
        {
            cooldownFill.fillAmount = 1;
            float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
        
            cooldownFill.fillAmount = normalizedCooldown;
        }
        
        if (isActive)
        {
            float normalizedDuration = slot.durationRemaining / slot.ability.duration;
        
            durationFill.fillAmount = 1 - normalizedDuration;
        }
        else
        {
            durationFill.fillAmount = 0;
        }
        
        if (slot.IsReady())
            isActive = false;
    }

    public void SetAbilityActive()
    {
        if (!isActive)
        {
            durationFill.fillAmount = 1;
            isActive = true;
        }
    }
}
