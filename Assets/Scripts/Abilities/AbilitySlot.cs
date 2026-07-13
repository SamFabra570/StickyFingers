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
    
    public bool IsReady => state == AbilityState.Ready;

    public bool IsOnCooldown => state == AbilityState.Cooldown;

    public bool IsActive => state == AbilityState.Active || state == AbilityState.Ending;

    public bool IsDisabled => state == AbilityState.Disabled;
    
    public void SetState(AbilityState newState)
    {
        if (state == newState) return;

        state = newState;
    }

    //Updates cooldown timer
    public void UpdateCooldown(float deltaTime)
    {
        //If cooldownRemaining, count down time every frame 
        if (IsOnCooldown)
        {
            cooldownRemaining -= deltaTime;

            //Cooldown done
            if (cooldownRemaining < 0f)
                SetReady();
        }
    }

    public void UpdateDuration(float deltaTime)
    {
        if (!IsActive)
            return;
        
        durationRemaining -= deltaTime;

        if (durationRemaining < durationThreshold && IsActive)
        {
            if (durationRemaining >= 0)
                AbilityManager.Instance.DeactivateAbility(this);
            else 
                BeginEnding();
        }
    }
    
    public void BeginActive()
    {
        durationRemaining = ability.duration;
        durationThreshold = ability.duration * ability.warningThreshold;
        
        SetState(AbilityState.Active);
    }

    public void BeginEnding()
    {
        SetState(AbilityState.Ending);
    }

    public void BeginCooldown()
    {
        durationRemaining = 0f;
        cooldownRemaining = ability.cooldown;
        
        SetState(AbilityState.Cooldown);
    }

    public void SetReady()
    {
        cooldownRemaining = 0f;
        
        SetState(AbilityState.Ready);
    }

    public void Disable()
    {
        durationRemaining = 0f;
        cooldownRemaining = 0f;
        
        SetState(AbilityState.Disabled);
    }
}
