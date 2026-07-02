using UnityEngine;

public class PassiveAbilityController : MonoBehaviour
{
    public Passive equippedPassive;
    public PassiveAbilities equippedPassiveState = PassiveAbilities.None;
    
    public void EquipPassive(Passive passive)
    {
        equippedPassive = passive;
        equippedPassiveState = equippedPassive.passiveID;
        
        Debug.Log("Equipped Passive: " + equippedPassive);
    }

    public void DequipPassive()
    {
        equippedPassive = null;
        equippedPassiveState = PassiveAbilities.None;
        
        Debug.Log("Dequipped passive");
    }

    public bool Has(PassiveAbilities currentPassive)
    {
        if (equippedPassiveState == PassiveAbilities.None)
            return false;
        
        return equippedPassiveState == currentPassive;
    }
}
