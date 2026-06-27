using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    
    public List<string> unlockedAbilities = new();
    public List<string> missionCompleteAbilities = new();

    public bool unlockingAbility;

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
    }
}
