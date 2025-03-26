using Domain;
using GameBrain;

namespace DAL;

public static class ConfigMapper
{
    public static Config GameConfigurationToConfig(GameConfiguration gameConfiguration)
    {
        var config = new Config
        {
            ConfigName = gameConfiguration.Name,
            BoardWidth = gameConfiguration.BoardWidth,
            BoardHeight = gameConfiguration.BoardHeight,
            WinCondition = gameConfiguration.WinCondition,
            AmountOfPieces = gameConfiguration.AmountOfPieces,
            MovePieceAndGridAfterNMoves = gameConfiguration.MovePieceAndGridAfterNMoves,
            GridWidth = gameConfiguration.GridWidth,
            GridHeight = gameConfiguration.GridHeight,
        };
        return config;
    }

    public static GameConfiguration ConfigToGameConfiguration(Config config)
    {
        var gameConfiguration = new GameConfiguration
        {
            Name = config.ConfigName,
            BoardWidth = config.BoardWidth,
            BoardHeight = config.BoardHeight,
            WinCondition = config.WinCondition,
            AmountOfPieces = config.AmountOfPieces,
            MovePieceAndGridAfterNMoves = config.MovePieceAndGridAfterNMoves,
            GridWidth = config.GridWidth,
            GridHeight = config.GridHeight,
        };
        return gameConfiguration;
    }
}