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
    //[SerializeField] private TextMeshProUGUI abilityIcon;
    //[SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Button unlockButton;
    
    private ProgressionManager progressionManager;

    private void Start()
    {
        unlockButton = GetComponent<Button>();
        abilityName = GetComponentInChildren<TextMeshProUGUI>();
        progressionManager = GameManager.Instance.GetComponent<ProgressionManager>();
        
        SetButtonUI();
        UpdateState();
        
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
        abilityLockOverlay.SetActive(false);
        
        UpdateState();
    }

    private void UpdateState()
    {
        bool unlocked = progressionManager.IsUnlocked(ability);
        bool canUnlock = progressionManager.CanUnlock(ability);

        unlockButton.interactable = canUnlock && !unlocked;
    }
}
