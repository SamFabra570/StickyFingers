using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PhaseAbility")]
public class PhaseAbility : Ability
{
    public float duration;
    
    public override void Activate(GameObject user)
    {
        ActivatePhase(user);
    }

    private void ActivatePhase(GameObject user)
    {
        //Add logic here
        Debug.Log("Phase Activated");
    }
    
}
