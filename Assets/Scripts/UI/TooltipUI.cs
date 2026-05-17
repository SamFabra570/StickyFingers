using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Tooltip Ref")] 
    [SerializeField] private GameObject tooltipObj;
    
    [Header ("UI Refs")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    
    [Header ("Positioning")]
    [SerializeField] private Vector2 tooltipOffset = new Vector2(250f, -7.5f);
    
    private RectTransform rectTransform;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();

        Hide();
    }

    public void Show(DraggableItem item, Transform selectedSlot)
    {
        if (item == null)
            return;
        
        //Move tooltip beside selected slot
        PositionTooltip(selectedSlot);
        
        tooltipObj.SetActive(true);
        
        UpdateTooltip(item);
    }

    public void Hide()
    {
        tooltipObj.SetActive(false);
    }

    private void UpdateTooltip(DraggableItem item)
    {
        Ability selectedAbility = item.ability.ability;
        
        //Abilities
        if (item.abilityType == AbilityType.Ability)
        {
            nameText.text = selectedAbility.abilityName;
            descriptionText.text = selectedAbility.abilityDescription;
            durationText.text = (selectedAbility.duration + "s");
            cooldownText.text = (selectedAbility.cooldown + "s");
        }
        
        //Passives
        if (item.abilityType ==  AbilityType.Passive)
        {
            nameText.text = ("" + item.passiveAbility);
            descriptionText.text = ("");
            durationText.text = ("Passive");
            cooldownText.text = ("");
        }
    }

    private void PositionTooltip(Transform selectedSlot)
    {
        RectTransform slotRect = selectedSlot.GetComponent<RectTransform>();
        
        rectTransform.position = slotRect.position + (Vector3)tooltipOffset;
    }
}
