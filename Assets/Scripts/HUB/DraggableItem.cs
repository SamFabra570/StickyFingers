using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AbilityType
{
    Ability,
    Passive
}

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AbilityType abilityType;
    
    [Header("Abilities")]
    public AbilitySlot ability;
    
    [Header("Passives")]
    public PassiveAbilities passiveAbility;
    
    [Header("UI")]
    public Image image;
    public Image iconImage;
    [HideInInspector] public Transform parentAfterDrag;

    private void Start()
    {
        image = GetComponent<Image>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}


