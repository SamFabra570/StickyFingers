using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PhaseAbility")]
public class PhaseAbility : Ability
{
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.rend.material =  abilityColour;
        PlayerController.Instance.gameObject.layer = LayerMask.NameToLayer("Intangible");
        Debug.Log("Phase Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.rend.material =  PlayerController.Instance.basePlayerMat;
        PlayerController.Instance.gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log("Phase Deactivated");
    }
    
}
