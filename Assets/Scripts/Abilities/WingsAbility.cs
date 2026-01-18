using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Wings")]
public class WingsAbility : Ability
{
    public float speedModifier;
    
    public override void Activate(GameObject user)
    {
        //Add logic here
        Debug.Log("Wings Activated");
    }
    
    public override void Deactivate(GameObject user)
    {
        //Deactivation logic here
        Debug.Log("Wings Deactivated");
    }
}
