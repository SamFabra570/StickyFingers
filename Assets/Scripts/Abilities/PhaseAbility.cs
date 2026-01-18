using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PhaseAbility")]
public class PhaseAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        //Add logic here
        Debug.Log("Phase Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        //Deactivation logic here
        Debug.Log("Phase Deactivated");
    }
    
}
