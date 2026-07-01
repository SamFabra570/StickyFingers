using UnityEngine;

public class PassiveAbilityController : MonoBehaviour
{    
    public Passive equippedPassive;
    
    public void EquipPassive(Passive passive)
    {
        equippedPassive = passive;
        
        Debug.Log("Equipped Passive: " + equippedPassive.passiveName);
    }

    public void DequipPassive()
    {
        equippedPassive = null;
        Debug.Log("Dequipped passive");
    }

    public bool Has(PassiveAbilities currentPassive)
    {
        if (equippedPassive.passiveID == PassiveAbilities.None)
            return false;
        
        return equippedPassive.passiveID == currentPassive;
    }
}
