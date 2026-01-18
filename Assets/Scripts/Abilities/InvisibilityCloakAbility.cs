using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        //Add logic here
        Debug.Log("Invisibility Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        //Deactivation logic here
        Debug.Log("Invisibility Deactivated");
    }
}
