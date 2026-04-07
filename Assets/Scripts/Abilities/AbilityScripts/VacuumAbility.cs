using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Vacuum")]
public class VacuumAbility : Ability
{
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.vacuumZone.gameObject.SetActive(true);
    }

    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.vacuumZone.gameObject.SetActive(false);
    }
}
