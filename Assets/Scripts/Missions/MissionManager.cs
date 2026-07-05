using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public MissionTemplateUI missionUI;
    
    public MissionData activeMission;
    public int currentAmount;
    
    public bool IsComplete => activeMission.currentAmount >= activeMission.requiredAmount;
    
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
        
        missionUI.activeMissionIndicator.SetActive(true);
        
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
        
        activeMission.currentAmount += amount;
        
        Debug.Log("Progress: " + (float) currentAmount/activeMission.requiredAmount);

        if (IsComplete)
             CompleteMission();
    }

    private void CompleteMission()
    {
        if (activeMission.rewardAbility != null)
        {
            ProgressionManager.Instance.UnlockAbility(activeMission.rewardAbility);
            //activeMission.rewardAbility.;
        }
        
        Debug.Log("Mission Complete!");
        activeMission = null;
    }
}
