using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
    
    [SerializeField] private float tooltipDelay = 0.75f;
    [SerializeField] private Canvas targetCanvas;

    private Coroutine tooltipCoroutine;

    private const int SelectedOrder = 200;
    
    public EventSystem eventSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        rectTransform = GetComponent<RectTransform>();

        Hide();
    }

    private void Show(DraggableItem item, Transform selectedSlot)
    {
        if (item == null)
            return;
        
        //Move tooltip beside selected slot
        PositionTooltip(selectedSlot);
        
        tooltipObj.SetActive(true);
        
        UpdateTooltip(item);
    }

    private void Hide()
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
    
    public void StartTooltip(GameObject tooltipTrigger)
    {
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);

        tooltipCoroutine = StartCoroutine(ShowTooltip(tooltipTrigger));
    }

    public void StopTooltip()
    {
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);
        
        ResetCanvasOrder();

        Hide();
    }

    private IEnumerator ShowTooltip(GameObject tooltipTrigger)
    {
        yield return new WaitForSeconds(tooltipDelay);
        
        DraggableItem item = tooltipTrigger.GetComponentInChildren<DraggableItem>();

        if (item == null)
        {
            yield break;
        }
        
        targetCanvas = tooltipTrigger.GetComponent<Canvas>();

        SetSelectedCanvasOrder();
        
        Show(item, tooltipTrigger.transform);
    }

    private void SetSelectedCanvasOrder()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponent<Canvas>();

        if (targetCanvas != null)
        {
            targetCanvas.overrideSorting = true;
            targetCanvas.sortingOrder = SelectedOrder;
        }
    }

    private void ResetCanvasOrder()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponent<Canvas>();

        if (targetCanvas != null)
        {
            targetCanvas.overrideSorting = false;
            //targetCanvas.sortingOrder = SelectedOrder;
        }
    }
}
