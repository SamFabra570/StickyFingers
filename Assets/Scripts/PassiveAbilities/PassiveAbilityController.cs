using UnityEngine;

public class PassiveAbilityController : MonoBehaviour
{    
    public PassivesUIData equippedPassive;
    
    public void EquipPassive(DraggableItem passive)
    {
        equippedPassive = new PassivesUIData
        {
            passiveAbility = passive.passiveAbility,
            icon = passive.iconImage.sprite,
            frameColor = passive.image.color
        };
        
        Debug.Log("Equipped Passive: " + equippedPassive.passiveAbility);
    }

    public bool Has(PassiveAbilities currentPassive)
    {
        if (equippedPassive.passiveAbility == PassiveAbilities.None)
            return false;
        
        return equippedPassive.passiveAbility == currentPassive;
    }
}
