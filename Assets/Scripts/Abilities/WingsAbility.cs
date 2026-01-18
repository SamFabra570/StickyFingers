using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Wings")]
public class WingsAbility : Ability
{
    public float duration;
    public float speedModifier;
    
    public override void Activate(GameObject user)
    {
        ActivateWings(user);
    }

    private void ActivateWings(GameObject user)
    {
        //Add logic here
        Debug.Log("Wings Activated");
    }
}
