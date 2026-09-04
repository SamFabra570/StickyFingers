using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public MissionTemplateUI missionUI;
    
    public MissionData activeMission;
    //public int currentAmount;
    
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
        if (ProgressionManager.Instance.IsUnlocked(mission.rewardAbility))
        {
            Debug.Log(mission.missionName + " is already complete!");
            return;
        }

        if (activeMission == mission)
        {
            missionUI.activeMissionIcon.SetActive(false);
            //missionUI.ShowMissionStatus(false);

            activeMission = null;
            
            Debug.Log("Deselected mission: " + mission.missionName);
            return;
        }
            
        activeMission = null;
        activeMission = mission;
        
        missionUI.activeMissionIcon.SetActive(true);
        //missionUI.ShowMissionStatus(true);
        
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
        
        Debug.Log("Progress: " + (float) activeMission.currentAmount/activeMission.requiredAmount);

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
