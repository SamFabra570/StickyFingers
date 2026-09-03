using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "Passive")]
public class Passive : ScriptableObject
{
    [Header("Passive")]
    public PassiveAbilities passiveID;
    public string passiveName;
    [TextArea]
    public string passiveDescription;
    public Material passiveColour;
    public Sprite passiveInventoryIcon;
    
    [Header("Progression")] 
    [Range(0f, 1f)]
    public float debtThreshold;
    
    public MissionData unlockMission;
    public bool unlockedByDefault;
}
