using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    
    public List<string> unlockedAbilities = new();
    public List<string> missionCompleteAbilities = new();

    public bool unlockingAbility;

    public bool isDirty = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    public bool IsMissionCompleted(Ability ability)
    {
        return missionCompleteAbilities.Contains(ability.abilityID);
    }
    
    public bool IsUnlocked(Ability ability)
    {
        if (ability.unlockedByDefault)
        {
            unlockedAbilities.Add(ability.abilityID);
            return true;
        }
        
        return unlockedAbilities.Contains(ability.abilityID);
    }

    public bool CanUnlock(Ability ability)
    {
        return GameManager.Instance.GetDebtPaidPercent() >= ability.debtThreshold;
    }

    public void UnlockAbility(Ability ability)
    {
        if (IsUnlocked(ability))
            return;
        
        unlockedAbilities.Add(ability.abilityID);
        unlockingAbility = true;
        
        Debug.Log(ability.abilityName + " unlocked");

        isDirty = true;
    }
}
