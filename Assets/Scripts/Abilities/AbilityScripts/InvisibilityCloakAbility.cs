using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.SetPlayerColour(abilityColour);
        PlayerController.Instance.isInvisible = true;
        
        PlayerController.Instance.playerVisualController.SetSpriteOpacity(0.6f);
        //Debug.Log("Invisibility Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.ResetColour();
        PlayerController.Instance.isInvisible = false;
        
        PlayerController.Instance.playerVisualController.SetSpriteOpacity(1f);
        //Debug.Log("Invisibility Deactivated");
    }
}
