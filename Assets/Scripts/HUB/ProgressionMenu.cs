using UnityEngine;

public class ProgressionMenu : MonoBehaviour, IUIMenu
{
    [SerializeField] private Canvas menuCanvas;

    public void OnShowMenu()
    {
        HUB_UIManager.Instance.detailsScreenAnim.SetBool("GTFO?", true);
        
        menuCanvas.overrideSorting = true;
        menuCanvas.sortingOrder = 100;
    }

    public void OnHideMenu()
    {
        HUB_UIManager.Instance.detailsScreenAnim.SetBool("GTFO?", false);
        
        menuCanvas.overrideSorting = false;
    }

    public void OnNavigate(Vector2 direction) { }

    public void OnSubmit() { }

    public void OnCancel() { }
}
