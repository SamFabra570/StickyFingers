using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AbilitySlot
{
    public Ability ability;
    public float cooldownRemaining;
    public float durationThreshold;
    public float durationRemaining;
    public bool isActive;

    public AbilityState state = AbilityState.Ready;

    //Returns true if off cooldown, false if still on cooldown
    public bool IsReady()
    {
        return state ==  AbilityState.Ready;
    }

    //Updates cooldown timer
    public void UpdateCooldown(float deltaTime)
    {
        //If cooldownRemaining, count down time every frame 
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= deltaTime;

            //Cooldown done
            if (cooldownRemaining < 0f)
            {
                state = AbilityState.Ready;
                cooldownRemaining = 0f;
            }
                
        }
    }

    public void StartDuration()
    {
        durationRemaining = ability.duration;
        durationThreshold = ability.duration * 0.3f;
        isActive = true;
    }

    public void UpdateDuration(float deltaTime)
    {
        if (!isActive)
            return;
        
        durationRemaining -= deltaTime;

        if (durationRemaining < durationThreshold)
        {
            state = AbilityState.Ending;
            Debug.Log("change state: " + state);
        }
        
        if (durationRemaining <= 0)
        {
            durationRemaining = 0f;
            state = AbilityState.Cooldown;
            isActive = false;
        }
        
        
    }
}
