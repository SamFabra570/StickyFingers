using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    
    [Header("Positioning")]
    [SerializeField] private Vector2 rightOffset = new Vector2(-20, 0f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-250f, -7.5f);
    
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

        tooltipObj.SetActive(true);

        Canvas.ForceUpdateCanvases();

        PositionTooltip(selectedSlot);

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
            if (!cooldownText.gameObject.activeSelf)
                cooldownText.gameObject.SetActive(true);
            
            nameText.text = selectedAbility.abilityName;
            descriptionText.text = selectedAbility.abilityDescription;
            durationText.text = (selectedAbility.duration + "s");
            cooldownText.text = (selectedAbility.cooldown + "s");
        }
        
        //Passives
        if (item.abilityType ==  AbilityType.Passive)
        {
            nameText.text = ("" + item.passiveAbility);
            descriptionText.text = ("" + item.description);
            durationText.text = ("Passive");
            cooldownText.gameObject.SetActive(false);
        }
    }

    private void PositionTooltip(Transform selectedSlot)
    {
        RectTransform slotRect = selectedSlot.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // convert slot position into screen space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, slotRect.position);

        // try right side first
        rectTransform.position = slotRect.position + (Vector3)rightOffset;

        Canvas.ForceUpdateCanvases();

        // get tooltip corners in SCREEN SPACE (important fix)
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float rightEdgeScreenX = RectTransformUtility.WorldToScreenPoint(null, corners[2]).x;

        // screen check
        if (rightEdgeScreenX > Screen.width)
        {
            rectTransform.position = slotRect.position + (Vector3)leftOffset;
        }
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
