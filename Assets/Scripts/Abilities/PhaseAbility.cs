using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PhaseAbility")]
public class PhaseAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.ActivatePhase();
        Debug.Log("Phase Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.DeactivatePhase();
        Debug.Log("Phase Deactivated");
    }
    
}
