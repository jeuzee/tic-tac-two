using ConsoleUI;
using DAL;
using Domain;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static TicTacTwoBrain? _gameInstance;
    private static EGameMode _gameMode;

    public static void MainLoop(GameState state, EGameMode mode)
    {
        _gameInstance = new TicTacTwoBrain(state);
        _gameMode = mode;
        MainLoop();
    }
    
    public static void MainLoop(TicTacTwoBrain gameInstance, EGameMode mode)
    {
        _gameInstance = gameInstance;
        _gameMode = mode;
        MainLoop();
    }
    
    private static void MainLoop()
    {
        if (_gameInstance == null) return;
        
        do
        {
            if (_gameInstance.WinColumn() == EGamePiece.X ||
                _gameInstance.WinRow() == EGamePiece.X ||
                _gameInstance.WinDiagonal() == EGamePiece.X)
            {
                Visualizer.DrawBoard(_gameInstance);
                Console.WriteLine("Player X won!");
                return;
            } 
            if (_gameInstance.WinColumn() == EGamePiece.O ||
                _gameInstance.WinRow() == EGamePiece.O || 
                _gameInstance.WinDiagonal() == EGamePiece.O)
            {
                Visualizer.DrawBoard(_gameInstance);
                Console.WriteLine("Player 0 won!");
                return;
            }
            if (_gameInstance.CheckTie())
            {
                Visualizer.DrawBoard(_gameInstance);
                Console.WriteLine("Tie!");
                return;
            }

            Visualizer.DrawBoard(_gameInstance);
            
            if (_gameInstance.GetNextMoveBy() == EGamePiece.X)
            {
                Console.Write("Player X turn \n");
            }
            else
            {
                Console.Write("Player O turn \n");
            }

            if ((_gameInstance.GetNextMoveBy() == EGamePiece.X && _gameMode == EGameMode.PlayerVsAi) ||
                _gameMode == EGameMode.PlayerVsPlayer)
            {
                var gameOptionsMenu = new Menu(
                    EMenuLevel.Game,
                    "Game Options",
                    OptionsController.GetOptionsList(_gameInstance),
                    isCustomMenu: true
                );
                
                var menuReturnValue = gameOptionsMenu.Run();
                
                if (menuReturnValue == "R")
                {
                    Console.WriteLine("Exiting game...");
                    return;
                }
            }
            else
            {
                Thread.Sleep(1000);
                _gameInstance.AiAction();
            }
        } while (true);
    }
}