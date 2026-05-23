using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressionMenu : MonoBehaviour, IUIMenu
{
    public static ProgressionMenu Instance;
    
    public EventSystem eventSystem;
    
    [SerializeField] private Canvas menuCanvas;
    
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject readyButton;

    private GameObject currentSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject != currentSelected &&  eventSystem.currentSelectedGameObject != null) 
            ValidateNavigation();
    }

    public void OnShowMenu()
    {
        HUB_UIManager.Instance.detailsScreenAnim.SetBool("GTFO?", true);
        
        menuCanvas.overrideSorting = true;
        menuCanvas.sortingOrder = 100;

        if (firstButton.activeSelf)
            eventSystem.SetSelectedGameObject(firstButton);
        else 
            eventSystem.SetSelectedGameObject(FindNextInteractable(firstButton));
    }

    public void OnHideMenu()
    {
        HUB_UIManager.Instance.detailsScreenAnim.SetBool("GTFO?", false);
        
        menuCanvas.overrideSorting = false;
    }

    private void ValidateNavigation()
    {
        var current = eventSystem.currentSelectedGameObject;
        
        if (current != null)
        {
            Debug.Log(current + "progression menu");
            
            var button = current.GetComponent<Button>();

            if (button != null && !button.interactable)
            {
                eventSystem.SetSelectedGameObject(currentSelected);
            }
            else 
                currentSelected =  button.gameObject;
        }
    }
    
    public void MoveSelectionAfterUnlock(GameObject selectedButton)
    {
        selectedButton = eventSystem.currentSelectedGameObject;

        eventSystem.SetSelectedGameObject(FindNextInteractable(selectedButton));
    }
    
    private GameObject FindNextInteractable(GameObject current)
    {
        var selectable = current.GetComponent<Selectable>();

        if (selectable != null)
        {
            var next = selectable.FindSelectableOnRight();

            if (next != null && next.interactable)
            {
                Debug.Log("next:" + next);
                return next.gameObject;
            }
            
            Debug.Log("selectable:" + selectable);
                
        }

        return readyButton;
    }

    public void OnNavigate(Vector2 direction) { }

    public void OnSubmit()
    {
        
    }

    public void OnCancel()
    {
        HUB_UIManager.Instance.TogglePlanningUI("Close");
    }
}
