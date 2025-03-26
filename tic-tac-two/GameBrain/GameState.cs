using Domain;

namespace GameBrain;

public class GameState
{
    public string PlayerX { get; set; } = "";
    public string PlayerO { get; set; } = "";
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    public int PiecesPlaced { get; set; }
    public int GridStartX { get; set; }
    public int GridStartY { get; set; }
    
    public int GridEndX { get; set; }
    
    public int GridEndY { get; set; }

    public string StateName { get; set; } = default!;
    
    public GameConfiguration GameConfiguration { get; set; }

    public List<AiGridMove> AiGridMoves { get; set; }
    
    public GameState(EGamePiece[][] gameBoard, GameConfiguration gameConfiguration)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;

        GridStartX = (gameConfiguration.BoardWidth - gameConfiguration.GridWidth) / 2;
        GridStartY = (gameConfiguration.BoardHeight - gameConfiguration.GridHeight) / 2;
        GridEndX = GridStartX + gameConfiguration.GridWidth - 1;
        GridEndY = GridStartY + gameConfiguration.GridHeight - 1;
        AiGridMoves = [];
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}