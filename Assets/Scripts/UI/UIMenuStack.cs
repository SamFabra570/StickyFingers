using System.Collections.Generic;

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
    }

    public static void Pop()
    {
        if (menuStack.Count == 0)
            return;
        
        IUIMenu top  = menuStack.Pop();
        top.OnHideMenu();
        
        if (Current != null)
            Current.OnShowMenu();
    }

    public static void Clear()
    {
        while (menuStack.Count > 0)
            menuStack.Pop().OnHideMenu();
    }
}
