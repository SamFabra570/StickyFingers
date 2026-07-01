using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Button passiveButton;
    
    private RectTransform rect;
    private Vector2 originalPos;
    
    [SerializeField] float hoverOffset = 20f;

    public Passive passive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();
        
        originalPos = rect.anchoredPosition;
        
        passiveButton = GetComponent<Button>();
        passiveButton.onClick.AddListener(SelectButton);
    }

    public void OnPointerEnter(PointerEventData eventData) => HoverButton();
    public void OnPointerExit(PointerEventData eventData) => DeselectButton();

    public void OnSelect(BaseEventData eventData) => HoverButton();
    public void OnDeselect(BaseEventData eventData) => DeselectButton();

    public void SelectButton()
    {
        Debug.Log("Selected");
        
        //transform.DOScale(1.2f, 0.2f);
        //rect.DOKill();
        
        Vector2 newPos = originalPos + Vector2.up * hoverOffset;
        
        //transform.DOLocalMoveY(transform.localPosition.y + 20f, 0.2f);
        //rect.DOAnchorPos(newPos, 1f).SetEase(Ease.OutBack);
        //rect.DOScale(1.05f, 0.2f);

        LoadoutMenu.Instance.SelectPassive(passive);
    }

    private void HoverButton()
    {
        
    }

    private void DeselectButton()
    {
        //Debug.Log("Deselected");
        
        //rect.DOKill();

        rect.DOAnchorPos(originalPos, 0.2f).SetEase(Ease.OutQuad);
    }
}
