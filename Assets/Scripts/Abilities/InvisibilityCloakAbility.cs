using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.SetPlayerColour(abilityColour);
        PlayerController.Instance.isInvisible = true;
        Debug.Log("Invisibility Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.ResetColour();
        PlayerController.Instance.isInvisible = false;
        Debug.Log("Invisibility Deactivated");
    }
}
