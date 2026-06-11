using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    
    public MissionData activeMission;
    public int currentAmount;
    
    private bool IsComplete => currentAmount >= activeMission.requiredAmount;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMission(MissionData mission)
    {
        activeMission = mission;
        currentAmount = 0;
        
        Debug.Log("Started mission: " + mission.missionName);
    }

    public void AddProgress(InventoryItemData item, int amount)
    {
        //If no active mission
        if (activeMission == null)
            return;

        //If item is wrong item
        if (activeMission.targetItem != item)
            return;
        
        currentAmount += amount;
        
        Debug.Log("Progress: " + (float) currentAmount/activeMission.requiredAmount);

        if (IsComplete)
            CompleteMission();
    }

    private void CompleteMission()
    {
        if (activeMission.rewardAbility != null)
        {
            ProgressionManager.Instance.missionCompleteAbilities.Add(activeMission.rewardAbility.abilityID);
        }
        
        Debug.Log("Mission Complete!");
        activeMission = null;
    }
}
