using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    //Controller
    public void OnSelect(BaseEventData eventData)
    {
        TooltipUI.Instance.StartTooltip(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        TooltipUI.Instance.StopTooltip();
    }

    //Mouse
     // public void OnPointerEnter(PointerEventData eventData)
     // {
     //     StartTooltip();
     // }
     //
     // public void OnPointerExit(PointerEventData eventData)
     // {
     //     StopTooltip();
     //     ResetCanvasOrder();
     // }

    //Tooltip logic
    
}
