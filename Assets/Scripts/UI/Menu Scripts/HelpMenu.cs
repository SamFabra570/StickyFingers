using UnityEngine;
using UnityEngine.EventSystems;

public class HelpMenu : MonoBehaviour, IUIMenu
{
    //[SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject helpMenuScreen;
    
    public void OnShowMenu()
    {
        helpMenuScreen.SetActive(true);
    }

    public void OnHideMenu()
    {
        helpMenuScreen.SetActive(false);
    }

    public void OnCancel()
    {
        UIManager.Instance.HideMenu();
    }
}
