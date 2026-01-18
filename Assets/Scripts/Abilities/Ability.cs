using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float duration;
    public float cooldown;

    public abstract void Activate(GameObject user);
    
    public abstract void Deactivate(GameObject user);
}
