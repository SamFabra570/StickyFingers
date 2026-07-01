using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Abilities")]
    public AbilitySlot ability;
    
    
    [Header("UI")]
    public Image iconImage;
    public string description;
    public Transform originalParent;
    [HideInInspector] public Transform parentAfterDrag;

    private void Start()
    {
        iconImage = GetComponent<Image>();
        //iconImage = transform.GetChild(0).GetComponent<Image>();
        originalParent = transform.parent;

        iconImage.sprite = ability.ability.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin drag");
        //TooltipUI.Instance.Hide();
        
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        //image.raycastTarget = false;
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
        //image.raycastTarget = true;
    }
}


