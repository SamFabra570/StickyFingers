using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.ActivateInvisibility();
        Debug.Log("Invisibility Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.DeactivateInvisibility();
        Debug.Log("Invisibility Deactivated");
    }
}
