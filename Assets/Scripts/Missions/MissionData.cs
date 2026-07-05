using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Mission Data")]
public class MissionData : ScriptableObject
{
    public string missionName;
    public string description;
    
    [Header ("Objective")]
    public InventoryItemData targetItem;
    public GameObject itemPrefab;
    public int requiredAmount;
    public int currentAmount;

    public Ability rewardAbility;


}
