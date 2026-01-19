using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shield")]
public class ShieldAbility : Ability
{
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.ActivateShield();
    }

    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.DeactivateShield();
    }
}
