using UnityEngine;

public class PassiveAbilityController : MonoBehaviour
{    
    public PassiveAbilities equippedPassive =  PassiveAbilities.None;
    
    public void EquipPassive(PassiveAbilities passive)
    {
        equippedPassive = passive;
        
        switch (equippedPassive)
        {
            case (PassiveAbilities.None):
                Debug.Log("No passive equipped");
                break;
            case (PassiveAbilities.SafetySlot):
                //Add logic
                Debug.Log("Safety Slot passive equipped");
                break;
            case (PassiveAbilities.EndlessPockets):
                //Add logic
                Debug.Log("Endless Pockets passive equipped");
                break;
            case (PassiveAbilities.SecondChance):
                //Add logic
                Debug.Log("Second Chance passive equipped");
                break;
            case (PassiveAbilities.ExtraTime):
                //Add logic
                Debug.Log("Extra Time passive equipped");
                break;
        }
    }

    public bool Has(PassiveAbilities currentPassive)
    {
        return equippedPassive == currentPassive;
    }
}
