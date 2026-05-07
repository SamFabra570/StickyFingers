using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("Ability")]
    public string abilityID;
    public string abilityName;
    
    public Sprite icon;
    public Material abilityColour;
    
    [Header("Progression")] 
    [Range(0f, 1f)]
    public float debtThreshold;
    
    public bool requiredUnlockItems;

    public string unlockItemID;
    public int unlockItemAmount;
    
    [Header("Gameplay")]
    public float duration;
    public float warningThreshold = 0.3f;
    public float cooldown;

    public abstract void Activate(GameObject user);
    
    public abstract void Deactivate(GameObject user);
}
