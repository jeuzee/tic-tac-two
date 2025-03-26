namespace MenuSystem;

public class Menu
{
    private string MenuHeader { get; set; }
    private static string _menuDivider = "================";
    private List<MenuItem> MenuItems { get; set; }

    private readonly MenuItem _menuItemExit = new MenuItem()
    {
        Shortcut = "E",
        Title = "Exit"
    };
    
    private readonly MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return"
    };
    private readonly MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "Return to main menu"
    };
    
    private EMenuLevel _menuLevel { get; set; }
    
    private bool _isCustomMenu { get; set; }
    
    public void SetMenuItemAction(string shortCut, Func<string> action)
    {
        var menuItem = MenuItems.Single(m => m.Shortcut == shortCut);
        menuItem.MenuItemAction = action;
    }
    
    public Menu(EMenuLevel menuLevel, string menuHeader, List<MenuItem> menuItems, bool isCustomMenu = false)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Header cannot be null or empty.");
        }
        
        MenuHeader = menuHeader;
        
        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be null or empty.");
        }
        
        MenuItems = menuItems;
        _isCustomMenu = isCustomMenu;
        _menuLevel = menuLevel;
        
        
        if (_menuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }
        
        if (_menuLevel == EMenuLevel.Deep && _menuLevel != EMenuLevel.Load)
        {
            MenuItems.Add(_menuItemReturnMain);
        }

        if (_menuLevel != EMenuLevel.Game)
        {
            MenuItems.Add(_menuItemExit);
        }
    }

    public string Run(int startX = 0, int startY = 0)
    {
        if (_menuLevel != EMenuLevel.Game &&
            _menuLevel != EMenuLevel.Load && !_isCustomMenu)
        {
            Console.Clear(); 
        }
        do
        {
            var menuItem = DisplayMenuGetUserChoice(startX, startY);
            var menuReturnValue = "";
            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction!();
                if (_isCustomMenu || _menuLevel == EMenuLevel.GameMode)
                {
                    return menuReturnValue;
                }
            }

            if (menuItem.Shortcut == _menuItemReturn.Shortcut || (menuItem.Title == "Delete" || menuItem.Title == "Continue"))
            {
                return _menuItemReturn.Shortcut;
            }
            
            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut)
            {
                return _menuItemExit.Shortcut;
            }

            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut || menuReturnValue == _menuItemReturnMain.Shortcut) && _menuLevel != EMenuLevel.Main)
            {
                return _menuItemReturnMain.Shortcut;
            }

        } while (true);
    }

    private MenuItem DisplayMenuGetUserChoice(int startX, int startY)
    {
        
        do
        {
            DrawMenu(startX, startY);
            
            var userInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("It would be nice, if you actually choose something!!! Try again... Maybe...");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();

                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }
                
                Console.WriteLine("Try to choose something from the existing options... Please....");
                Console.WriteLine();
            }
        } while (true);
    }

    // startX, startY to draw game menu next to the board
    private void DrawMenu(int startX, int startY)
    {
        // Console.SetCursorPosition(startX, startY);
        
        Console.WriteLine(MenuHeader);
        // Console.SetCursorPosition(startX, ++startY);
        Console.WriteLine(_menuDivider);
        
        foreach (var t in MenuItems)
        {
            // Console.SetCursorPosition(startX, ++startY);
            Console.WriteLine(t);
        }
        
        Console.WriteLine();
        // Console.SetCursorPosition(startX, ++startY);
        Console.Write(">");
    }
    
    public void UpdateMenuItemTitle(int index, string newTitle)
    {
        if (index >= 0 && index < MenuItems.Count)
        {
            MenuItems[index].Title = newTitle;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid menu item index.");
        }
    }
}