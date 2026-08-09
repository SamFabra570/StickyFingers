using UnityEngine;

public interface IUIMenu
{
    void OnShowMenu();
    void OnHideMenu();

    void OnNavigate(Vector2 input) { }

    void OnSubmit() { }

    void OnCancel() { }

    void OnButtonNorth() { }
    void OnButtonEast() { }
    void OnButtonSouth() { }
    void OnButtonWest() { }
}
