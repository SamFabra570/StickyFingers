using System;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Tooltip Ref")] 
    [SerializeField] private GameObject tooltipObj;
    
    [Header ("UI Refs")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    //[SerializeField] private TextMeshProUGUI durationText;
    //[SerializeField] private TextMeshProUGUI cooldownText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Hide();
    }

    public void Show(DraggableItem item)
    {
        if (item == null)
            return;
        
        Ability selectedAbility = item.ability.ability;
        
        nameText.text = selectedAbility.abilityName;
        descriptionText.text = selectedAbility.abilityDescription;
        
        //If Ability (passives have no duration/cooldown
        if (item.abilityType == AbilityType.Ability)
        {
            //durationText.text = (selectedAbility.duration + "s");
            //cooldownText.text = (selectedAbility.cooldown + "s");
        }
    }

    public void Hide()
    {
        tooltipObj.SetActive(false);
    }
}
