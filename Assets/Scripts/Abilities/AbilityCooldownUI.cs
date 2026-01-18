using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Range (0,2)]
    public int slotIndex;
    
    public Image cooldownFill;

    // Update is called once per frame
    void Update()
    {
        AbilitySlot slot = AbilityManager.Instance.GetAbility(slotIndex);

        if (slot.ability.cooldown <= 0f)
        {
            cooldownFill.fillAmount = 0;
            return;
        }
        
        if (slot.ability == null)
        {
            cooldownFill.fillAmount = 0;
            return;
        }

        float normalizedCooldown = slot.cooldownRemaining / slot.ability.cooldown;
        
        cooldownFill.fillAmount = normalizedCooldown;
        //Debug.Log(slot.cooldownRemaining);
    }
}
