using DAL;
using Domain;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;
    
    private static GameConfiguration _currentConfig;
    private static GameState _currentState = default!;
    private static string _stateName = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    public static IGameRepository GetGameRepository()
    {
        return _gameRepository;
    }
    
    /*
     * Edit config menu:
     * 1. change title,
     * 2. change board width and height,
     * 3. change grid size,
     * 4. change amount of pieces for players,
     * 5. change required amount of pieces to win,
     * 6. change required amount of pieces to move grid/piece,
     * 7. delete config.
     */
    private static readonly Menu EditConfigMenu = new Menu(
        EMenuLevel.Deep,
        "Edit TIC-TAC-TWO configuration",
        [
            new MenuItem
            {
                Title = "Change Title",
                Shortcut = "1",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new configuration name (currently - {_configRepository.ConfigNameToDisplay(_currentConfig.Name)}):");
                    var oldName = _currentConfig.Name;
                    var newName = Console.ReadLine();
                    while (newName == null || newName.Equals("") || newName.Contains('@'))
                    {
                        if (newName == null || newName.Equals(""))
                        {
                            Console.WriteLine("Enter something.");
                        } else if (newName.Contains('@'))
                        {
                            Console.WriteLine("Name can't contain this symbol: '@'");
                        }
                        newName = Console.ReadLine();
                    }
                    _currentConfig.Name = newName ?? _currentConfig.Name;
                    _currentConfig.Name = _configRepository.SaveConfig(_currentConfig, oldName).Result;
                    Console.WriteLine("Name updated!");
                    return "1";
                }
            },
            new MenuItem
            {
                Title = "Change Board size",
                Shortcut = "2",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new board width (currently - {_currentConfig.BoardWidth}):");
                    if (int.TryParse(Console.ReadLine(), out var newWidth))
                    {
                        if (newWidth >= 3 && newWidth <= 20)
                        {
                            Menus._currentConfig.BoardWidth = newWidth;
                        }
                    }
                    
                    Console.WriteLine($"Enter new board height (currently {_currentConfig.BoardHeight}):");
                    if (int.TryParse(Console.ReadLine(), out var newHeight))
                    {
                        if (newHeight >= 3 && newHeight <= 20)
                        {
                            Menus._currentConfig.BoardHeight = newHeight;
                        }
                    }
                    _configRepository.SaveConfig(_currentConfig, _currentConfig.Name);
                    Console.WriteLine("Board size updated!");
                    return "2";
                }
            },
            new MenuItem
            {
                Title = "Change Grid size",
                Shortcut = "3",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new grid width (currently - {_currentConfig.GridWidth}):");
                    if (int.TryParse(Console.ReadLine(), out var newWidth))
                    {
                        if (newWidth >= 3 && newWidth <= 20 && newWidth <= _currentConfig.BoardWidth && newWidth >= _currentConfig.WinCondition)
                        {
                            Menus._currentConfig.GridWidth = newWidth;
                        }
                    }
                    
                    Console.WriteLine($"Enter new grid height (currently - {_currentConfig.GridHeight}):");
                    if (int.TryParse(Console.ReadLine(), out var newHeight))
                    {
                        if (newHeight >= 3 && newHeight <= 20 && newHeight <= _currentConfig.BoardHeight && newHeight >= _currentConfig.WinCondition)
                        {
                            Menus._currentConfig.GridHeight = newHeight;
                        }
                    }

                    if (newWidth == 0 && newHeight == 0)
                    {
                        Menus._currentConfig.GridWidth = newWidth;
                        Menus._currentConfig.GridHeight = newHeight;
                    }
                    
                    _configRepository.SaveConfig(_currentConfig, _currentConfig.Name);
                    Console.WriteLine("Grid size updated!");
                    return "3";
                }
            },
            new MenuItem
            {
                Title = "Change Amount of pieces for players",
                Shortcut = "4",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new amount of pieces for players (currently - {_currentConfig.AmountOfPieces}):");
                    if (int.TryParse(Console.ReadLine(), out var newAmount))
                    {
                        if (newAmount >= 3)
                        {
                            Menus._currentConfig.AmountOfPieces = newAmount;
                        }
                    }
                    _configRepository.SaveConfig(_currentConfig, _currentConfig.Name);
                    Console.WriteLine("Amount of pieces for players updated!");
                    return "4";
                }
            },
            new MenuItem
            {
                Title = "Change Required amount of pieces to win in a row/column/diagonal",
                Shortcut = "5",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new required amount of pieces in a row/column/diagonal to win (currently - {_currentConfig.WinCondition}):");
                    if (int.TryParse(Console.ReadLine(), out var newAmount))
                    {
                        if (newAmount >= 3 
                            && newAmount <= _currentConfig.BoardWidth 
                            && newAmount <= _currentConfig.BoardHeight
                            && newAmount <= _currentConfig.AmountOfPieces)
                        {
                            if ((_currentConfig.GridWidth == 0 && _currentConfig.GridHeight == 0) 
                                || (newAmount <= _currentConfig.GridWidth && newAmount <= _currentConfig.GridHeight))
                            {
                                Menus._currentConfig.WinCondition = newAmount;
                            }
                        }
                    }
                    _configRepository.SaveConfig(_currentConfig, _currentConfig.Name);
                    Console.WriteLine("Amount of pieces for players updated!");
                    return "5";
                }
            },
            new MenuItem
            {
                Title = "Change Required amount of placed pieces to move grid/piece",
                Shortcut = "6",
                MenuItemAction = () =>
                {
                    Console.WriteLine($"Enter new required amount of placed pieces to move for players (currently {_currentConfig.MovePieceAndGridAfterNMoves}):");
                    if (int.TryParse(Console.ReadLine(), out var newAmount))
                    {
                        if (newAmount >= 2)
                        {
                            Menus._currentConfig.MovePieceAndGridAfterNMoves = newAmount;
                        }
                    }
                    _configRepository.SaveConfig(_currentConfig, _currentConfig.Name);
                    Console.WriteLine("Required amount of placed pieces to move grid/piece updated!");
                    return "6";
                }
            },
            new MenuItem
            {
                Title = "Delete",
                Shortcut = "7",
                MenuItemAction = () =>
                {
                    var configNameToDelete = _currentConfig.Name;
                    _configRepository.DeleteConfig(configNameToDelete);
                    Console.WriteLine("Config deleted!");
                    return "R";
                }
            },
        ]);
    
    /*
     * Main menu.
     */
    public static readonly Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO",
        MainMenuOptions()
        );

    /*
     * Main menu options:
     * 1. start new game,
     * 2. enter options menu to create/change/delete configs,
     * 3. enter load menu to continue or delete saved game.
     */
    private static List<MenuItem> MainMenuOptions()
    {
        List<MenuItem> mainMenu =
        [
            new MenuItem
            {
                Title = "New Game",
                Shortcut = "N",
                MenuItemAction = GetConfig
            },
            new MenuItem
            {
                Title = "Options",
                Shortcut = "O",
                MenuItemAction = GetOptions
            }
        ];
        
        mainMenu.Add(new MenuItem
        {
            Title = "Load Games",
            Shortcut = "L",
            MenuItemAction = GetSavedGames
        });    
        

        return mainMenu;
    }
    
    /*
     * Get "Create new config" choice and all existing configs and run menu with them.
     */
    private static string GetOptions()
    {
        var configMenuItems = new List<MenuItem>();
        configMenuItems.Add(new MenuItem
        {
            Title = "Create new config",
            Shortcut = "1",
            MenuItemAction = () => 
            {
                Console.WriteLine("Creating a new configuration.");
                _currentConfig = new GameConfiguration
                {
                    Name = "New config"
                };
                _currentConfig.Name = _configRepository.SaveConfig(_currentConfig, "New config").Result;
                
                var shortCut = EditConfigMenu.Run();
                if (shortCut == "R") GetOptions();
                return shortCut;
            }
        });

        for (int i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var title = _configRepository.GetConfigurationNames()[i];
            configMenuItems.Add(new MenuItem
            {
                Title = "Change configurations: " + _configRepository.ConfigNameToDisplay(title),
                Shortcut = (i + 2).ToString(),
                MenuItemAction = () =>
                {
                    _currentConfig = _configRepository.GetConfigurationByName(title);
                    var shortCut = EditConfigMenu.Run();
                    if (shortCut == "R") GetOptions();
                    return shortCut;
                }
            });
        }
        
        var optionsMenu = new Menu(
                EMenuLevel.Secondary,
                "TIC-TAC-TWO Options",
                configMenuItems,
                true
            );
        
        return optionsMenu.Run();
    }

    /*
     * Menu with choice to continue or delete saved game.
     */
    private static readonly Menu ContinueOrDelete = new Menu(
        EMenuLevel.Deep,
        "Continue saved game or delete",
        [new MenuItem 
            {
                Title = "Continue",
                Shortcut = "1",
                MenuItemAction = () =>
                {
                    var mode = GetMode();
                    GameController.MainLoop(_currentState, mode);
                    return "R";
                }
            },
            new MenuItem
            {
                Title = "Delete",
                Shortcut = "2",
                MenuItemAction = () =>
                {
                    _gameRepository.DeleteGame(_stateName);
                    Console.WriteLine("Game deleted!");
                    return "";
                }
            }
        ]
    );


    private static List<MenuItem> GetSavedGamesList()
    {
        var configMenuItems = new List<MenuItem>();
        for (int i = 0; i < _gameRepository.GetGameNames().Count; i++)
        {
            var title = _gameRepository.GetGameNames()[i];
            configMenuItems.Add(new MenuItem
            {
                Title = _gameRepository.GameNameToDisplay(title),
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () =>
                {
                    _stateName = title;
                    _currentState = _gameRepository.GetGameByName(title);
                    _currentState.StateName = title;
                    var shortCut = ContinueOrDelete.Run();
                    if (shortCut == "R") GetSavedGames();
                    return shortCut;
                }
            });
        }

        return configMenuItems;
    }
    
    
    /*
     * Get saved games and run menu with them.
     */
    private static string GetSavedGames()
    {
        var configMenuItems = GetSavedGamesList();
        
        if (configMenuItems.Count == 0)
        {
            configMenuItems.Add(new MenuItem
            {
                Title = "No saved games",
                Shortcut = "B",
                MenuItemAction = () => MainMenu.Run()
            });
            var noGamesMenu = new Menu(EMenuLevel.Load,
                "No saved games",
                configMenuItems,
                true);
            return noGamesMenu.Run();
        }
        
        var savedGamesMenu = new Menu(EMenuLevel.Load,
            "Saved games",
            configMenuItems,
            isCustomMenu: true
        );

        return savedGamesMenu.Run();
        
    }

   
    /// <summary>
    /// Choose game mode.
    /// </summary>
    private static readonly Menu ChooseGameMode = new Menu(
        EMenuLevel.GameMode,
        "Choose game mode",
        [new MenuItem 
            {
                Title = "Player vs player",
                Shortcut = "1",
                MenuItemAction = () => "0"
            },
            new MenuItem
            {
                Title = "Player vs AI",
                Shortcut = "2",
                MenuItemAction = () => "1"
            },
            new MenuItem
            {
                Title = "AI vs AI",
                Shortcut = "3",
                MenuItemAction = () => "2"
            }
        ]
    );
    
    /*
     * Run menu with all existing configs.
     */
    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (int i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem
            {
                Title = _configRepository.ConfigNameToDisplay(_configRepository.GetConfigurationNames()[i]),
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems,
            isCustomMenu: true
        );

        return configMenu.Run();
    }

    /*
     * Get config and start game.
     */
    public static string GetConfig()
    {
        var chosenConfigShortcut = "";
        var wrongInput = true;
        TicTacTwoBrain gameInstance = default!;
        do
        {
            chosenConfigShortcut = ChooseConfiguration();
            if (chosenConfigShortcut is "R" or "E") return chosenConfigShortcut;

            if (int.TryParse(chosenConfigShortcut, out var configNo))
            {
                gameInstance = new TicTacTwoBrain(
                    _configRepository.GetConfigurationByName(
                        _configRepository.GetConfigurationNames()[configNo]));
                wrongInput = false;
            }
        } while (wrongInput);

        var mode = GetMode();
        GameController.MainLoop(gameInstance, mode);
        return "";
    }

    private static EGameMode GetMode()
    {
        var mode = EGameMode.PlayerVsPlayer;
        mode = ChooseGameMode.Run() switch
        {
            "1" => EGameMode.PlayerVsAi,
            "2" => EGameMode.AiVsAi,
            _ => mode
        };
        return mode;
    }
}