using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private List<string> unlockedAbilities = new();

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
        
        Debug.Log(ability.abilityName + " unlocked");
    }
}
