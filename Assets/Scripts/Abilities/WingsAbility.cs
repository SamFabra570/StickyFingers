using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Wings")]
public class WingsAbility : Ability
{
    public float flyHeight = 2f;
    public float flySpeed = 10f;
    
    public override void Activate(GameObject user)
    {
        PlayerController.Instance.useGravity = false;
        PlayerController.Instance.yVelocity = 0f;
        PlayerController.Instance.MovePlayerUp(flyHeight);
        
        PlayerController.Instance.abilityMoveSpeed = flySpeed;
        PlayerController.Instance.wings.SetActive(true);
        PlayerController.Instance.SetPlayerColour(abilityColour);
        //Debug.Log("Wings Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        PlayerController.Instance.useGravity = true;
        PlayerController.Instance.abilityMoveSpeed = 0;
        PlayerController.Instance.wings.SetActive(false);
        PlayerController.Instance.ResetColour();
        //Debug.Log("Wings Deactivated");
    }
}
