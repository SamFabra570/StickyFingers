using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionTemplateUI : MonoBehaviour
{
    [Header("Template References")] 
    public GameObject lockedTemplate;
    public GameObject unlockedTemplate;
    
    [Header ("Locked Template")]
    [SerializeField] private Image lockedIcon;
    //[SerializeField] private Image lockedIconColor;
    [SerializeField] private TextMeshProUGUI lockedAbilityNameText;
    public Slider progressionSlider;
    //[SerializeField] private GameObject lockOverlay;
    
    [Header ("Unlocked Template")]
    [SerializeField] private Image icon;
    //[SerializeField] private Image iconColor;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("Task Requirements")] 
    public GameObject activeMissionIndicator;
    public GameObject abilityUnlockedOverlay;
    [SerializeField] private GameObject requirementPanel;
    [SerializeField] private TextMeshProUGUI requirementDescriptionText;
    //[SerializeField] private TextMeshProUGUI requirementItemName;
    [SerializeField] private TextMeshProUGUI progressText;

    private void UpdateLockedPanel(AbilityUnlock abilityUnlock)
    {
        lockedAbilityNameText.text = abilityUnlock.ability.abilityName;
        lockedIcon.sprite = abilityUnlock.ability.icon;
        //lockedIconColor.color = abilityUnlock.ability.abilityColour.color;

        progressionSlider.value = GetProgressionSliderFill(abilityUnlock);
    }

    private void UpdateUnlockedPanel(AbilityUnlock abilityUnlock)
    {
        abilityNameText.text = abilityUnlock.ability.abilityName;
        icon.sprite = abilityUnlock.ability.icon;
        //iconColor.color = abilityUnlock.ability.abilityColour.color;
        descriptionText.text = abilityUnlock.ability.abilityDescription;
        durationText.text = ("" + abilityUnlock.ability.duration);
        cooldownText.text = ("" + abilityUnlock.ability.cooldown);
        
        UpdateRequirements(abilityUnlock);
    }

    private void UpdateRequirements(AbilityUnlock abilityUnlock)
    {
        if (abilityUnlockedOverlay.activeSelf) 
            abilityUnlockedOverlay.SetActive(false);
        
        if (ProgressionManager.Instance.IsUnlocked(abilityUnlock.ability)) //If ability is fully unlocked
        {
            abilityUnlockedOverlay.SetActive(true);
            return;
        }
        
        requirementDescriptionText.text = abilityUnlock.ability.unlockMission.description;

        if (MissionManager.Instance.activeMission == abilityUnlock.ability.unlockMission)
        {
            if (!MissionManager.Instance.IsComplete)
            {
                progressText.text = (abilityUnlock.ability.unlockMission.currentAmount + " / " + abilityUnlock.ability.unlockMission.requiredAmount);
                activeMissionIndicator.SetActive(true);
            }
        }
        else
        {
            activeMissionIndicator.SetActive(false);
            progressText.text = ("0 / " + abilityUnlock.ability.unlockMission.requiredAmount);
        }
    }
    
    private float GetProgressionSliderFill(AbilityUnlock abilityUnlock)
    {
        float unlockThreshold = abilityUnlock.ability.debtThreshold * GameManager.Instance.maxDebt;
        
        float normalizedDebt = ((GameManager.Instance.maxDebt - GameManager.Instance.remainingDebt) / unlockThreshold);

        if (normalizedDebt >= 1)
            return 1;
        
        return normalizedDebt;
    }
    
    public void UpdateMissionPanel(GameObject selectedAbilityUnlock)
    {
        AbilityUnlock abilityUnlock =  selectedAbilityUnlock.GetComponent<AbilityUnlock>();
        abilityUnlock.UpdateState();

        if (abilityUnlock == null)
        {
            Debug.LogWarning("No AbilityUnlock selected");
            return;
        }
        
        if (!ProgressionManager.Instance.CanUnlock(abilityUnlock.ability)) //If ability mission is locked
        {
            Debug.Log("Ability mission is locked");
            
            if (unlockedTemplate.activeSelf)
                unlockedTemplate.SetActive(false);
            if (!lockedTemplate.activeSelf) 
                lockedTemplate.SetActive(true);
            
            UpdateLockedPanel(abilityUnlock);
        }
        else if (ProgressionManager.Instance.CanUnlock(abilityUnlock.ability)) //If ability mission is unlocked
        {
            Debug.Log("Ability mission is unlocked");
            
            if (lockedTemplate.activeSelf)
                lockedTemplate.SetActive(false);
            if (!unlockedTemplate.activeSelf) 
                unlockedTemplate.SetActive(true);
            
            UpdateUnlockedPanel(abilityUnlock);
        }
        
    }
}
