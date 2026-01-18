using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/InvisibilityCloak")]
public class InvisibilityCloakAbility : Ability
{
    public float duration;
    
    public override void Activate(GameObject user)
    {
        ActivateInvisibility(user);
    }

    private void ActivateInvisibility(GameObject user)
    {
        //Add logic here
        Debug.Log("Invisibility Activated");
    }
}
