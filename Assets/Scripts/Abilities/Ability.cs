using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public Material abilityColour;
    public float duration;
    public float warningThreshold = 0.3f;
    public float cooldown;

    public abstract void Activate(GameObject user);
    
    public abstract void Deactivate(GameObject user);
}
