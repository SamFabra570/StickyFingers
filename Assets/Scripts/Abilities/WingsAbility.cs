using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Wings")]
public class WingsAbility : Ability
{
    public float flyHeight = 2f;
    public float flySpeed = 10f;
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.heightOffset = flyHeight;
        PlayerController.Instance.speed = flySpeed;
        PlayerController.Instance.wings.SetActive(true);
        PlayerController.Instance.SetPlayerColour(abilityColour);
        Debug.Log("Wings Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.heightOffset = 0;
        PlayerController.Instance.speed = 5f;
        PlayerController.Instance.wings.SetActive(false);
        PlayerController.Instance.ResetColour();
        Debug.Log("Wings Deactivated");
    }
}
