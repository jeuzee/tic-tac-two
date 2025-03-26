namespace GameBrain;

public record struct GameConfiguration()
{
    public string Name { get; set; } = default!;

    public int BoardWidth { get; set; } = 3;
    public int BoardHeight { get; set; } = 3;
    public int WinCondition { get; set; } = 3;
    public int AmountOfPieces { get; set; } = 5;
    public int MovePieceAndGridAfterNMoves { get; set; } = 0;

    public int GridWidth { get; set; } = 0;
    public int GridHeight { get; set; } = 0;

    public override string ToString()
    {
        return
            $"Board {BoardWidth}x{BoardHeight}, to win: {WinCondition}" +
            $" pieces in a row/column/diagonal inside grid {GridWidth}x{GridHeight}," +
            $" can move pieces or grid after {MovePieceAndGridAfterNMoves} moves," +
            $" total amount of pieces for each player: {AmountOfPieces}";
    }
}