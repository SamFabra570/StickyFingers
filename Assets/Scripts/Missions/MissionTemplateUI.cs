using System.Collections;
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
    [SerializeField] private TextMeshProUGUI lockedAbilityNameText;
    public Slider progressionSlider;
    
    [Header ("Unlocked Template")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [SerializeField] private GameObject abilityUnlockedImage;

    [Header("Mission Requirements")] 
    [SerializeField] private GameObject missionBox;
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI missionDescriptionText;
    
    [SerializeField] public GameObject activeMissionIcon;
    [SerializeField] private GameObject missionStatusCheck;
    
    [SerializeField] private Image missionItemIcon;
    [SerializeField] private TextMeshProUGUI progressText;
    
    [SerializeField] public TextMeshProUGUI missionStatusText;
    
    private Coroutine missionStatusCoroutine;
    

    private void UpdateLockedPanel(AbilityUnlock abilityUnlock)
    {
        lockedAbilityNameText.text = abilityUnlock.ability.abilityName;
        lockedIcon.sprite = abilityUnlock.ability.icon;

        //Fix this to go back to slider values per ability
        //progressionSlider.value = GetProgressionSliderFill(abilityUnlock);
        progressionSlider.value = GameManager.Instance.GetDebtPaidPercent();
    }

    private void UpdateUnlockedPanel(AbilityUnlock abilityUnlock)
    {
        abilityNameText.text = abilityUnlock.ability.abilityName;
        icon.sprite = abilityUnlock.ability.icon;
        
        descriptionText.text = abilityUnlock.ability.abilityDescription;
        durationText.text = ("" + abilityUnlock.ability.duration);
        cooldownText.text = ("" + abilityUnlock.ability.cooldown);
        
        UpdateMissionRequirements(abilityUnlock);
    }

    private void UpdateMissionRequirements(AbilityUnlock abilityUnlock)
    {
        if (ProgressionManager.Instance.IsUnlocked(abilityUnlock.ability)) //If ability is fully unlocked
        {
            abilityUnlockedImage.SetActive(true);
            
            missionBox.SetActive(false);
            return;
        }
        
        abilityUnlockedImage.SetActive(false);
        missionBox.SetActive(true);
        
        missionNameText.text = abilityUnlock.ability.unlockMission.missionName;
        missionDescriptionText.text = abilityUnlock.ability.unlockMission.description;
    
        if (MissionManager.Instance.activeMission == abilityUnlock.ability.unlockMission)
        {
            if (!MissionManager.Instance.IsComplete)
            {
                progressText.text = (abilityUnlock.ability.unlockMission.currentAmount + " / " + abilityUnlock.ability.unlockMission.requiredAmount);
                activeMissionIcon.SetActive(true);
            }
        }
        else
        {
            activeMissionIcon.SetActive(false);
            progressText.text = (abilityUnlock.ability.unlockMission.currentAmount + " / " + abilityUnlock.ability.unlockMission.requiredAmount);
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
        
        if (!ProgressionManager.Instance.IsMissionAvailable(abilityUnlock.ability)) //If ability mission is locked
        {
            Debug.Log("Ability mission is locked");
            
            if (unlockedTemplate.activeSelf)
                unlockedTemplate.SetActive(false);
            if (!lockedTemplate.activeSelf) 
                lockedTemplate.SetActive(true);
            
            UpdateLockedPanel(abilityUnlock);
        }
        else if (ProgressionManager.Instance.IsMissionAvailable(abilityUnlock.ability)) //If ability mission is unlocked
        {
            //Debug.Log("Ability mission is unlocked");
            
            if (lockedTemplate.activeSelf)
                lockedTemplate.SetActive(false);
            if (!unlockedTemplate.activeSelf) 
                unlockedTemplate.SetActive(true);
            
            UpdateUnlockedPanel(abilityUnlock);
        }
        
    }

    public void ShowMissionStatus(bool isActive)
    {
        if (missionStatusCoroutine != null)
            StopCoroutine(missionStatusCoroutine);
        
        missionStatusCoroutine = StartCoroutine(ShowMissionStatusText(isActive));
    }
    
    private IEnumerator ShowMissionStatusText(bool isActive)
    {
        if (isActive)
            missionStatusText.text = "Mission activated!";
        else
            missionStatusText.text = "Mission deactivated!";
        
        missionStatusText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f);

        missionStatusText.gameObject.SetActive(false);
        
        missionStatusCoroutine = null;
    }
}
