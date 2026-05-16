using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float tooltipDelay = 0.75f;

    private Coroutine tooltipCoroutine;

    public void OnSelect(BaseEventData eventData)
    {
        tooltipCoroutine = StartCoroutine(ShowTooltip());
    }

    public void OnDeselect(BaseEventData eventData)
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
        
        TooltipUI.Instance.Show(item);
    }
}
