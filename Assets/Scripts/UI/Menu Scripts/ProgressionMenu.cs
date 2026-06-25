using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressionMenu : MonoBehaviour, IUIMenu
{
    public static ProgressionMenu Instance;

    private LoadoutMenu loadoutMenu;
    [SerializeField] private MissionTemplateUI missionUI;
    
    public EventSystem eventSystem;
    
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas HUBCanvas;
    
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject readyButton;

    private GameObject lastSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        loadoutMenu = UIManager.Instance.loadoutMenu;
    }

    private void Update()
    {
        if (ReferenceEquals(UIMenuStack.Current, this))
        {
            if (eventSystem.currentSelectedGameObject != lastSelected && eventSystem.currentSelectedGameObject != null)
            {
                ValidateNavigation();
            }
        }
    }

    public void OnShowMenu()
    {
        HUBCanvas.enabled = true;
        
        //loadoutMenu.detailsScreenAnim.SetBool("GTFO?", true);
        
        menuCanvas.overrideSorting = true;
        menuCanvas.sortingOrder = 100;

        if (firstButton.activeSelf)
            eventSystem.SetSelectedGameObject(firstButton);
        else 
            eventSystem.SetSelectedGameObject(FindNextInteractable(firstButton));
    }

    public void OnHideMenu()
    {
        //loadoutMenu.detailsScreenAnim.SetBool("GTFO?", false);
        
        menuCanvas.overrideSorting = false;
        
        HUBCanvas.enabled = false;
    }

    //Check if newly selected button is navigable, if not, return to last selected button
    private void ValidateNavigation()
    {
        var current = eventSystem.currentSelectedGameObject;
        
        if (current != null)
        {
            var button = current.GetComponent<Button>();

            if (button != null && !button.interactable)
            {
                eventSystem.SetSelectedGameObject(lastSelected);
            }
            else
            {
                lastSelected =  button.gameObject;
                missionUI.UpdateMissionPanel(lastSelected);
            }
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
                if (next.gameObject.activeSelf)
                    return next.gameObject;
                    
                return readyButton;
            }
        }

        return readyButton;
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
}
