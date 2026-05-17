using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float tooltipDelay = 0.75f;
    [SerializeField] private Canvas targetCanvas;

    private Coroutine tooltipCoroutine;

    private const int SelectedOrder = 200;
    //private const int DefaultOrder = 0;

    //Controller
    public void OnSelect(BaseEventData eventData)
    {
        StartTooltip();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StopTooltip();
        ResetCanvasOrder();
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
    private void StartTooltip()
    {
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);

        tooltipCoroutine = StartCoroutine(ShowTooltip());
    }

    private void StopTooltip()
    {
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);

        TooltipUI.Instance.Hide();
    }

    private IEnumerator ShowTooltip()
    {
        yield return new WaitForSeconds(tooltipDelay);

        if (EventSystem.current.currentSelectedGameObject != gameObject)
            yield break;
        
        DraggableItem item = GetComponentInChildren<DraggableItem>();

        if (item == null)
            yield break;

        SetSelectedCanvasOrder();
        
        TooltipUI.Instance.Show(item, transform);
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
