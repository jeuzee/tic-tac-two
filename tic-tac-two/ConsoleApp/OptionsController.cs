using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class OptionsController
{
    public static List<MenuItem> GetOptionsList(TicTacTwoBrain gameInstance)
    {
        var optionsList = new List<MenuItem>
        {
            new MenuItem
            {
                Title = "Make Move",
                Shortcut = "1",
                MenuItemAction = () =>
                {
                    MakeMove(gameInstance!);
                    return "1";
                }
            },
        };
        
        if (gameInstance.ActionsAllowed != 0 && gameInstance.PiecesPlaced >= gameInstance.ActionsAllowed)
        {
            optionsList.Add(
                new MenuItem
                {
                    Title = "Move Piece",
                    Shortcut = "2",
                    MenuItemAction = () =>
                    {
                        MovePiece(gameInstance);
                        return "2";
                    }
                });
            optionsList.Add(
                new MenuItem
                {
                    Title = "Move Grid",
                    Shortcut = "3",
                    MenuItemAction = () =>
                    {
                        MoveGrid(gameInstance);
                        return "3";
                    }
                });
            optionsList.Add(
                new MenuItem
                {
                    Title = "Save Game",
                    Shortcut = "4",
                    MenuItemAction = () =>
                    {
                        SaveGame(gameInstance);
                        return "4";
                    }
                });
            optionsList.Add(
                new MenuItem
                {
                    Title = "Reset Game",
                    Shortcut = "5",
                    MenuItemAction = () =>
                    {
                        gameInstance.ResetGame();
                        return "5";
                    }
                });
        }
        else
        {
            optionsList.Add(
                new MenuItem
                {
                    Title = "Save Game",
                    Shortcut = "2",
                    MenuItemAction = () =>
                    {
                        SaveGame(gameInstance);
                        return "2";
                    }
                });
            optionsList.Add(
                new MenuItem
                {
                    Title = "Reset Game",
                    Shortcut = "3",
                    MenuItemAction = () =>
                    {
                        gameInstance.ResetGame();
                        return "3";
                    }
                });
        }
        return optionsList;
    }
    
    private static void MakeMove(TicTacTwoBrain gameInstance)
    {
        Console.Write($"Player {gameInstance.GetNextMoveBy()} - move <x,y>: ");
        var input = Console.ReadLine()!;
        var inputs = gameInstance.ValidateInput(input);
        if (inputs.Count() == 2)
        {
            int inputX = inputs[0];
            int inputY = inputs[1];
            gameInstance.MakeAMove(inputX, inputY);
        }
    }

    private static void MovePiece(TicTacTwoBrain gameInstance)
    {
        if (gameInstance.PiecesPlaced < gameInstance.ActionsAllowed)
        {
            Console.WriteLine("Not enough pieces placed to move the piece!");
            return;
        }

        Console.Write($"Write <oldX,oldY,newX,newY> coordinates to move '{gameInstance.GetNextMoveBy()}' piece:");
        var movePiece = Console.ReadLine()!;
        var inputs = gameInstance.ValidateInput(movePiece);
        if (inputs.Count() == 4)
        {
            int oldX = inputs[0];
            int oldY = inputs[1];
            int newX = inputs[2];
            int newY = inputs[3];

            gameInstance.MovePiece(oldX, oldY, newX, newY);
        }
    }

    private static void MoveGrid(TicTacTwoBrain gameInstance)
    {
        if (gameInstance.PiecesPlaced < gameInstance.ActionsAllowed)
        {
            Console.WriteLine("Not enough pieces placed to move the grid!");
            return;
        }

        Console.Write($"Player {gameInstance.GetNextMoveBy()} - Write new <x,y> start coordinates:");
        var newCoordinates = Console.ReadLine()!;
        var inputs = gameInstance.ValidateInput(newCoordinates);
        if (inputs.Count() == 2)
        {
            int newX = inputs[0];
            int newY = inputs[1];
            gameInstance.MoveGrid(newX, newY);
        }
    }

    private static void SaveGame(TicTacTwoBrain gameInstance)
    {
        var savedGameId = Menus.GetGameRepository().SaveGame(
            gameInstance.SetGameStateJson(),
            gameInstance.GetGameConfigName(),
            gameInstance.GetGameStateName()
        );
        
        gameInstance.SetStateName(Menus.GetGameRepository().LoadGame(savedGameId.Result).Result!.GameName);
    }
}