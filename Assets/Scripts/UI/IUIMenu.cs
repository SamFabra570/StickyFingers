using UnityEngine;

public interface IUIMenu
{
    void OnShowMenu();
    void OnHideMenu();
    
    void OnNavigate(Vector2 direction);
    void OnSubmit();
    void OnCancel();
}
