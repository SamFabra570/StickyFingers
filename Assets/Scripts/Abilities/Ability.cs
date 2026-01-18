using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float cooldown;

    public abstract void Activate(GameObject user);
}
