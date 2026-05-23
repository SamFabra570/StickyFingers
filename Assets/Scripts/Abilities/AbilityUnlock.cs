using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUnlock : MonoBehaviour
{
    [Header("Ability")] 
    [SerializeField] private Ability ability;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private GameObject abilityLockOverlay;
    [SerializeField] private Button abilityButton;
    //[SerializeField] private TextMeshProUGUI abilityIcon;
    //[SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Button unlockButton;
    
    private ProgressionManager progressionManager;

    private void Start()
    {
        unlockButton = GetComponent<Button>();
        abilityName = GetComponentInChildren<TextMeshProUGUI>();
        progressionManager = ProgressionManager.Instance;
        
        SetButtonUI();
        UpdateState();
        
        //Debug.Log(progressionManager.unlockedAbilities.Count);
        
        unlockButton.onClick.AddListener(UnlockAbility);
    }

    private void Update()
    {
        UpdateState();
    }

    private void SetButtonUI()
    {
        abilityName.text = ability.abilityName;
        //abilityIcon.sprite = ability.icon;
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
        
        bool unlocked = progressionManager.IsUnlocked(ability);
        bool canUnlock = progressionManager.CanUnlock(ability);
            
        unlockButton.interactable = canUnlock;
        abilityLockOverlay.SetActive(!unlocked);
        abilityButton.interactable = unlocked;
        gameObject.SetActive(!unlocked);
    }
}
