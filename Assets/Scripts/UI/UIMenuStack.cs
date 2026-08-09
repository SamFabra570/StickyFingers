using System.Collections.Generic;
using UnityEngine;

public static class UIMenuStack
{
    private static Stack<IUIMenu> menuStack = new Stack<IUIMenu>();

    public static IUIMenu Current
    {
        get
        {
            if (menuStack.Count == 0)
                return null;
            
            return menuStack.Peek();
        }
    }

    public static void Push(IUIMenu menu)
    {
        if (Current != null)
            Current.OnHideMenu();
        
        menuStack.Push(menu);
        menu.OnShowMenu();
        
        UIManager.Instance.ToggleInteractText(false, "");
        
        //PlayerController.Instance.playerInput.SwitchCurrentActionMap("UI");

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.inputMap.Player.Disable();
            PlayerController.Instance.inputMap.UI.Enable();
        }
        
        //Debug.Log($"Push: {menu.GetType().Name}");
        //Debug.Log($"Stack Count: {menuStack.Count}");
    }
    
    public static void PushOverlay(IUIMenu menu)
    {
        menuStack.Push(menu);
        menu.OnShowMenu();
        
        UIManager.Instance.ToggleInteractText(false, "");
        
        //PlayerController.Instance.playerInput.SwitchCurrentActionMap("UI");

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.inputMap.Player.Disable();
            PlayerController.Instance.inputMap.UI.Enable();
        }
    }

    public static void Pop()
    {
        if (menuStack.Count == 0)
            return;
        
        IUIMenu top  = menuStack.Pop();
        top.OnHideMenu();

        if (Current != null)
        {
            Current.OnShowMenu();
        }
        
        //Debug.Log($"Pop: {top.GetType().Name}");
        //Debug.Log($"Stack Count: {menuStack.Count}");
    }
    
    public static void PopOverlay()
    {
        if (menuStack.Count == 0)
            return;

        IUIMenu top = menuStack.Pop();
        top.OnHideMenu();
    }

    public static void Clear()
    {
        while (menuStack.Count > 0)
            menuStack.Pop().OnHideMenu();
        
        //PlayerController.Instance.playerInput.SwitchCurrentActionMap("Player");
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.inputMap.UI.Disable();
            PlayerController.Instance.inputMap.Player.Enable();
        }
        
        //Debug.Log("Clearing menu stack");
        //UIManager.Instance.ToggleInteractText(false, "");
    }
}
