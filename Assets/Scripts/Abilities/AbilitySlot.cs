using UnityEngine;

[System.Serializable]
public class AbilitySlot
{
    public Ability ability;
    public float cooldownRemaining;
    public bool isActive;

    //Returns true if off cooldown, false if still on cooldown
    public bool IsReady()
    {
        return cooldownRemaining <= 0f && !isActive;
    }

    //Updates cooldown timer
    public void UpdateCooldown(float deltaTime)
    {
        //If cooldownRemaining, count down time every frame 
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= deltaTime;

            if (cooldownRemaining < 0f)
                cooldownRemaining = 0f;
        }
    }
}
