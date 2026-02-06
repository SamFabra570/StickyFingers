using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AbilitySlot
{
    public Ability ability;
    public AbilityState state = AbilityState.Ready;
    
    public float cooldownRemaining;
    public float durationThreshold;
    public float durationRemaining;
    public bool isActive;

    //Returns true if off cooldown, false if still on cooldown
    public bool IsReady()
    {
        return state ==  AbilityState.Ready;
    }

    //Updates cooldown timer
    public void UpdateCooldown(float deltaTime)
    {
        //If cooldownRemaining, count down time every frame 
        if (state == AbilityState.Cooldown)
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
        state  = AbilityState.Active;
    }

    public void UpdateDuration(float deltaTime)
    {
        if (!isActive)
            return;
        
        durationRemaining -= deltaTime;

        if (durationRemaining < durationThreshold && state == AbilityState.Active)
        {
            state = AbilityState.Ending;
        }
        
        if (durationRemaining <= 0 && state != AbilityState.Cooldown)
        {
            durationRemaining = 0f;
            AbilityManager.Instance.DeactivateAbility(this);
            isActive = false;
        }
    }
}
