using UnityEngine;

public interface IUIMenu
{
    void OnShowMenu();
    void OnHideMenu();
    
    void OnSubmit();
    void OnCancel();
}
