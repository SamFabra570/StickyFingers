using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.rend.material =  abilityColour;
        PlayerController.Instance.isInvisible = true;
        Debug.Log("Invisibility Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.rend.material =  PlayerController.Instance.basePlayerMat;
        PlayerController.Instance.isInvisible = false;
        Debug.Log("Invisibility Deactivated");
    }
}
