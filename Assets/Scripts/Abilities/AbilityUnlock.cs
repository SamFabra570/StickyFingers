using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUnlock : MonoBehaviour
{
    [Header("Ability")] 
    public Ability ability;
    //public MissionData mission;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private GameObject abilityLockOverlay;
    [SerializeField] private Button abilityButton;
    //[SerializeField] private TextMeshProUGUI abilityIcon;
    //[SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Button unlockButton;
    
    public bool unlocked;
    public bool canUnlock;
    
    private ProgressionManager progressionManager;

    private void Start()
    {
        unlockButton = GetComponent<Button>();
        abilityName = GetComponentInChildren<TextMeshProUGUI>();
        progressionManager = ProgressionManager.Instance;
        
        SetButtonUI();
        //UpdateState();
        
        //Debug.Log(progressionManager.unlockedAbilities.Count);
        
        //unlockButton.onClick.AddListener(StartMission);
    }

    private void Update()
    {
        if (ProgressionManager.Instance.unlockingAbility)
        {
            if (ProgressionManager.Instance.IsUnlocked(ability))
            {
                abilityLockOverlay.SetActive(!unlocked);
                ProgressionManager.Instance.unlockingAbility = false;
                Debug.Log(ability.abilityName + " overlay disabled, ability unlocked");
            }
        }
    }

    private void SetButtonUI()
    {
        abilityName.text = ability.abilityName;
        //abilityIcon.sprite = ability.icon;
    }

    private void StartMission()
    {
        MissionManager.Instance.StartMission(ability.unlockMission);
    }

    private void UnlockAbility()
    {
        progressionManager.UnlockAbility(ability);
        //Debug.Log(progressionManager.unlockedAbilities.Count);
        
        ProgressionMenu.Instance.MoveSelectionAfterUnlock(unlockButton.gameObject);

        UpdateState();
    }

    public void UpdateState()
    {
        if (progressionManager == null)
        {
            progressionManager = ProgressionManager.Instance;

            if (progressionManager == null)
            {
                Debug.LogWarning("ProgressionManager still null");
                return;
            }
        }
        
        unlocked = progressionManager.IsUnlocked(ability);
        canUnlock = progressionManager.CanUnlock(ability);
        
        if (canUnlock)
            unlockButton.onClick.AddListener(StartMission);
        else
        {
            Debug.Log("Mission is locked!");
        }
        
        abilityLockOverlay.SetActive(!unlocked);
            
        //unlockButton.interactable = canUnlock;
        //abilityLockOverlay.SetActive(!unlocked);
        abilityButton.interactable = unlocked;
        //gameObject.SetActive(!unlocked);
    }
}
