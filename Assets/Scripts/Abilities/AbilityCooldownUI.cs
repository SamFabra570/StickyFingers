using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Range (0,2)]
    public int slotIndex;

    private bool isActive;
    
    public Image cooldownFill;

    // Update is called once per frame
    void Update()
    {
        AbilitySlot slot = AbilityManager.Instance.GetAbility(slotIndex);

        if (slot.cooldownRemaining > 0)
        {
            float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
        
            cooldownFill.fillAmount = normalizedCooldown;
        }
        
        if (slot.IsReady())
            isActive = false;
    }

    public void SetAbilityActive()
    {
        if (!isActive)
        {
            cooldownFill.fillAmount = 1;
            isActive = true;
        }
    }
}
