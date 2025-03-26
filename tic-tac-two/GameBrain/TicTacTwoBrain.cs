using Domain;

namespace GameBrain;

public class TicTacTwoBrain
{
    private GameState GameState { get; set; }

    private int GridX => GameState.GameConfiguration.GridWidth;
    private int GridY => GameState.GameConfiguration.GridHeight;

    public int GridStartX { get; set; }
    public int GridStartY { get; set; }
    public int GridEndX { get; set; }
    public int GridEndY { get; set; }

    public int DimX => GameState.GameBoard.Length;
    public int DimY => GameState.GameBoard[0].Length;

    public int ActionsAllowed => GameState.GameConfiguration.MovePieceAndGridAfterNMoves;

    public int PiecesPlaced = 0;

    private List<AiGridMove> AiGridMoves = [];

    public TicTacTwoBrain(GameState gameState)
    {
        GameState = gameState;

        this.GridStartX = GameState.GridStartX;
        this.GridStartY = GameState.GridStartY;
        this.GridEndX = GameState.GridEndX;
        this.GridEndY = GameState.GridEndY;
        this.PiecesPlaced = GameState.PiecesPlaced;
        this.AiGridMoves = GameState.AiGridMoves;
    }

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardHeight];
        }

        GameState = new GameState(
            gameBoard,
            gameConfiguration
        );

        this.GridStartX = GameState.GridStartX;
        this.GridStartY = GameState.GridStartY;
        this.GridEndX = GameState.GridEndX;
        this.GridEndY = GameState.GridEndY;
    }

    public string GetPlayerX()
    {
        return GameState.PlayerX;
    }
    
    public string SetPlayerX(string name)
    {
        return GameState.PlayerX = name;
    }
    
    public string GetPlayerO()
    {
        return GameState.PlayerO;
    }
    
    public string SetPlayerO(string name)
    {
        return GameState.PlayerO = name;
    }

    public string SetGameStateJson()
    {
        GameState.GridStartX = this.GridStartX;
        GameState.GridStartY = this.GridStartY;
        GameState.GridEndX = this.GridEndX;
        GameState.GridEndY = this.GridEndY;
        GameState.PiecesPlaced = this.PiecesPlaced;
        GameState.AiGridMoves = this.AiGridMoves;

        return GameState.ToString();
    }

    public string GetGameConfigName()
    {
        return GameState.GameConfiguration.Name;
    }

    public string GetGameStateName()
    {
        return GameState.StateName;
    }

    public void SetStateName(string newName)
    {
        this.GameState.StateName = newName;
    }

    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        private set => GameState.GameBoard = value;
    }

    private int WinCondition => GameState.GameConfiguration.WinCondition;

    public EGamePiece WinColumn()
    {
        for (int x = 0; x < DimX; x++)
        {
            var countX = 0;
            var countO = 0;
            for (int y = 0; y < DimY; y++)
            {
                if (!IsInsideGrid(x, y)) continue;

                if (GameState.GameBoard[x][y] == EGamePiece.X)
                {
                    countX += 1;
                    countO = 0;
                }
                else if (GameState.GameBoard[x][y] == EGamePiece.O)
                {
                    countO += 1;
                    countX = 0;
                }

                if (countX == WinCondition) return EGamePiece.X;
                if (countO == WinCondition) return EGamePiece.O;

                if (GameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    countX = 0;
                    countO = 0;
                }
            }
        }

        return EGamePiece.Empty;
    }

    public EGamePiece WinRow()
    {
        for (int y = 0; y < DimY; y++)
        {
            var countX = 0;
            var countO = 0;
            for (int x = 0; x < DimX; x++)
            {
                if (!IsInsideGrid(x, y)) continue;

                if (GameState.GameBoard[x][y] == EGamePiece.X)
                {
                    countX += 1;
                    countO = 0;
                }
                else if (GameState.GameBoard[x][y] == EGamePiece.O)
                {
                    countO += 1;
                    countX = 0;
                }

                if (countX == WinCondition) return EGamePiece.X;
                if (countO == WinCondition) return EGamePiece.O;

                if (GameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    countX = 0;
                    countO = 0;
                }
            }
        }

        return EGamePiece.Empty;
    }

    public EGamePiece WinDiagonal()
    {
        // Check both directions at once (main diagonal and anti-diagonal)
        for (int startRow = 0; startRow < DimY; startRow++)
        {
            for (int startCol = 0; startCol < DimX; startCol++)
            {
                // Check main diagonal (top-left to bottom-right)
                if (IsValidDiagonalStart(startRow, startCol, 1, 1) &&
                    CheckConsecutivePieces(startRow, startCol, 1, 1))
                {
                    return GameState.GameBoard[startRow][startCol];
                }

                // Check anti-diagonal (top-right to bottom-left)
                if (IsValidDiagonalStart(startRow, startCol, 1, -1) &&
                    CheckConsecutivePieces(startRow, startCol, 1, -1))
                {
                    return GameState.GameBoard[startRow][startCol];
                }
            }
        }

        return EGamePiece.Empty;
    }

    // Ensure the starting position has enough space for a diagonal
    private bool IsValidDiagonalStart(int row, int col, int rowDir, int colDir)
    {
        int endRow = row + (WinCondition - 1) * rowDir;
        int endCol = col + (WinCondition - 1) * colDir;

        // Check if diagonal stays within board dimensions
        if (endRow < 0 || endRow >= DimY || endCol < 0 || endCol >= DimX) return false;

        // Check if diagonal stays within the grid (if defined)
        for (int i = 0; i < WinCondition; i++)
        {
            int newRow = row + i * rowDir;
            int newCol = col + i * colDir;

            if (!IsInsideGrid(newRow, newCol))
            {
                return false;
            }
        }

        return true;
    }

    // Check for consecutive pieces along any diagonal (main or anti)
    private bool CheckConsecutivePieces(int startRow, int startCol, int rowDir, int colDir)
    {
        EGamePiece firstPiece = GameState.GameBoard[startRow][startCol];

        // Ignore empty cells
        if (firstPiece == EGamePiece.Empty) return false;

        // Traverse in the given direction (rowDir, colDir) and check if all pieces match
        for (int i = 1; i < WinCondition; i++)
        {
            int newRow = startRow + i * rowDir;
            int newCol = startCol + i * colDir;

            // Ensure the newRow and newCol are within the actual game board dimensions
            if (newRow < 0 || newRow >= DimY || newCol < 0 || newCol >= DimX) return false;

            // Check if we're still inside the bounds
            if (!IsInsideGrid(newRow, newCol)) return false;

            // If any piece doesn't match the first, return false
            if (GameState.GameBoard[newRow][newCol] != firstPiece)
            {
                return false;
            }
        }

        // If we find 'winCondition' matching pieces, return true
        return true;
    }

    public bool CheckTie()
    {
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (GameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    return false; // if EGamePiece.Empty found, then there is space for more pieces.
                }
            }
        }

        return true;
    }

    private bool IsInsideGrid(int x, int y)
    {
        if (GridX == 0 || GridY == 0)
        {
            return true;
        }

        return x >= GridStartX && x <= GridEndX && y >= GridStartY && y <= GridEndY;
    }

    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[GameState.GameBoard.GetLength(0)][];

        for (var x = 0; x < GameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new EGamePiece[GameState.GameBoard[x].Length];
            for (var y = 0; y < GameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = GameState.GameBoard[x][y];
            }
        }

        return copyOfBoard;
    }

    public bool MakeAMove(int x, int y, bool movingPiece = false)
    {
        if (!movingPiece)
        {
            var placed = 0;
            for (int col = 0; col < DimX; col++)
            {
                for (int row = 0; row < DimY; row++)
                {
                    if (GameState.GameBoard[col][row] == this.GetNextMoveBy())
                    {
                        placed++;
                        if (placed == GameState.GameConfiguration.AmountOfPieces)
                        {
                            Console.WriteLine("You have no pieces left :(");
                            return false;
                        }
                    }
                }
            }
        }

        if (!ValidateCoordinates(x, y))
        {
            Console.WriteLine("Invalid coordinates!");
            return false;
        }

        if (GameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            Console.WriteLine("Cell is already occupied!");
            return false;
        }

        if (!IsInsideGrid(x, y))
        {
            Console.WriteLine("Move is outside the grid bounds!");
            return false;
        }

        GameState.GameBoard[x][y] = this.GetNextMoveBy();

        // flip the next piece
        FlipNextPiece();
        PiecesPlaced++;
        return true;
    }

    public void MoveGrid(int newGridStartX, int newGridStartY)
    {
        if (!ValidateCoordinates(newGridStartX, newGridStartY))
        {
            Console.WriteLine("Invalid coordinates (move grid).");
            return;
        }

        if (DimX - newGridStartX < GridX || DimY - newGridStartY < GridY)
        {
            Console.WriteLine("Can't move grid that much!");
            return;
        }

        if (newGridStartX == GridStartX && newGridStartY == GridStartY)
        {
            Console.WriteLine("There is no point in moving grid to same position where it is right now. Try again.");
            return;
        }

        GridStartX = newGridStartX;
        GridStartY = newGridStartY;
        GridEndX = GridStartX + GridX - 1;
        GridEndY = GridStartY + GridY - 1;
        FlipNextPiece();
    }

    public void MovePiece(int oldX, int oldY, int newX, int newY)
    {
        if (ActionsAllowed == 0)
        {
            Console.WriteLine("Action not allowed with property < 2");
            return;
        }

        if (!ValidateCoordinates(oldX, oldY) || !ValidateCoordinates(newX, newY))
        {
            Console.WriteLine("Invalid coordinates (move piece).");
            return;
        }

        var checkPiece = GameState.GameBoard[oldX][oldY];
        if (checkPiece == EGamePiece.Empty)
        {
            Console.WriteLine("Can't move nothing");
            return;
        }

        if (checkPiece != this.GetNextMoveBy())
        {
            Console.WriteLine("Can't move other player piece!");
            return;
        }

        if (PiecesPlaced < ActionsAllowed)
        {
            Console.WriteLine("Action not allowed yet.");
            return;
        }

        if (MakeAMove(newX, newY, true))
        {
            GameState.GameBoard[oldX][oldY] = EGamePiece.Empty;
        }
    }

    private void FlipNextPiece()
    {
        GameState.NextMoveBy = GameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }

    public EGamePiece GetNextMoveBy()
    {
        return GameState.NextMoveBy;
    }

    private bool ValidateCoordinates(int x, int y)
    {
        return x < DimX && y < DimY && x >= 0 && y >= 0;
    }

    public List<int> ValidateInput(string input)
    {
        var inputs = new List<int>();
        var inputSplit = input.Split(",");

        foreach (var inp in inputSplit)
        {
            if (int.TryParse(inp, out var convertedInput))
            {
                inputs.Add(convertedInput);
            }
        }

        return inputs;
    }

    public void ResetGame()
    {
        var gameBoard = new EGamePiece[GameState.GameConfiguration.BoardWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[GameState.GameConfiguration.BoardHeight];
        }

        GameState.GameBoard = gameBoard;
        GameState.NextMoveBy = EGamePiece.X;

        GridStartX = (DimX - GridX) / 2;
        GridStartY = (DimY - GridY) / 2;
        GridEndX = GridStartX + GridX - 1;
        GridEndY = GridStartY + GridY - 1;

        PiecesPlaced = 0;
        AiGridMoves = [];

        GameState.AiGridMoves = AiGridMoves;
        GameState.PiecesPlaced = PiecesPlaced;
    }

    public void AiAction()
    {
        var (action, oldX, oldY, newX, newY, newGridStartX, newGridStartY) = DecideNextAction();

        if (action.Equals("PlacePiece"))
        {
            MakeAMove(oldX, oldY);
        }
        else if (action.Equals("MovePiece"))
        {
            MovePiece(oldX, oldY, newX!.Value, newY!.Value);
        }
        else
        {
            MoveGrid(newGridStartX!.Value, newGridStartY!.Value);
            AiGridMoves.Add(new AiGridMove
            {
                GridStartX = newGridStartX.Value,
                GridStartY = newGridStartY.Value
            });
        }

        SetGameStateJson();
    }

    private (string action, int oldX, int oldY, int? newX, int? newY, int? newGridX, int? newGridY) DecideNextAction()
    {
        // Default action is to place a piece
        string chosenAction = "PlacePiece";
        int bestScore = int.MinValue;
        (int x, int y) bestMove = (-1, -1);
        (int oldX, int oldY, int newX, int newY) bestPieceMove = (-1, -1, -1, -1);
        (int gridStartX, int gridStartY) bestGridShift = (0, 0);
        // Check if only placing pieces is allowed

        if (PiecesPlaced == 0)
        {
            bestMove = GetRandomMove();
            return ("PlacePiece", bestMove.x, bestMove.y, null, null, null, null);
        }
        
        if (CountOpponentWinningMovesInsideGrid() > 1)
        {
            // Shift the grid to disrupt the opponent's winning paths.
            bestGridShift = FindBestGridShift();
            return ("ShiftGrid", -1, -1, null, null, bestGridShift.gridStartX, bestGridShift.gridStartY);
        }

        // Prioritize placing pieces first
        if (GetPlayersPlacedPiecesAmount(GameState.NextMoveBy) < GameState.GameConfiguration.AmountOfPieces)
        {
            // Focus only on placing pieces
            for (int y = 0; y < DimY; y++)
            {
                for (int x = 0; x < DimX; x++)
                {
                    if (GameState.GameBoard[x][y] != EGamePiece.Empty || !IsInsideGrid(x, y))
                    {
                        continue;
                    }

                    // Simulate placing a piece
                    GameState.GameBoard[x][y] = GameState.NextMoveBy;
                    int moveScore = EvaluateMove(x, y);
                    GameState.GameBoard[x][y] = EGamePiece.Empty;
                    if (moveScore > bestScore)
                    {
                        bestScore = moveScore;
                        bestMove = (x, y);
                    }
                }
            }

            if (bestScore == -1)
            {
                if (PiecesPlaced == 1)
                {
                    bestMove = GetRandomMove();
                }
                else
                {
                    bestMove = GetMoveNextToExistingOwnPiece();
                }
            }

            if (IsInsideGrid(bestMove.x, bestMove.y))
            {
                return ("PlacePiece", bestMove.x, bestMove.y, null, null, null, null);
            }
        }

        // Once ActionsAllowed threshold is reached, consider other actions
        // 1. Move a piece
        var piecesOutsideGrid = GetPiecesOutsideGrid(GameState.NextMoveBy);
        if (piecesOutsideGrid.Count > 0)
        {
            var oldCoords = piecesOutsideGrid.First();
            for (int y = GridStartY; y <= GridEndY; y++)
            {
                for (int x = GridStartX; x <= GridEndX; x++)
                {
                    if (GameState.GameBoard[x][y] != EGamePiece.Empty)
                    {
                        continue;
                    }
                    var simulation = SimulateMovingPiece(oldCoords.x, oldCoords.y,
                        x, y,
                        bestScore,
                        chosenAction,
                        bestPieceMove);

                    bestScore = simulation.bestScore;
                    bestPieceMove = (simulation.oldX, simulation.oldY, simulation.newX, simulation.newY);
                    chosenAction = simulation.choosenAction;

                }
            }
            if (bestScore == -1)
            {
                var moveNextToExistingOwnPiece = GetMoveNextToExistingOwnPiece();
                bestPieceMove = (oldCoords.x, oldCoords.y, moveNextToExistingOwnPiece.x, moveNextToExistingOwnPiece.y);
                chosenAction = "MovePiece";
            }

        }
        else
        {
            for (int y = 0; y < DimY; y++)
            {
                for (int x = 0; x < DimX; x++)
                {
                    if (GameState.GameBoard[x][y] == GameState.NextMoveBy)
                    {
                        (int newX, int newY) newCoords = GetMoveNextToExistingOwnPiece();
                        var simulation = SimulateMovingPiece(x, y,
                            newCoords.newX, newCoords.newY,
                            bestScore,
                            chosenAction,
                            bestPieceMove);

                        bestScore = simulation.bestScore;
                        bestPieceMove = (simulation.oldX, simulation.oldY, simulation.newX, simulation.newY);
                        chosenAction = simulation.choosenAction;
                    }
                }
            }
        }

        if (bestPieceMove != (-1, -1, -1, -1) && CountSelfWinningMovesInsideGrid() > 0)
        {
            return (chosenAction, bestPieceMove.oldX, bestPieceMove.oldY, bestPieceMove.newX,
                bestPieceMove.newY, null, null);
        }
        
        // 2. Shift the grid
        var (bestShiftX, bestShiftY) = FindBestGridShift();
        if (bestShiftX != 0 || bestShiftY != 0) // Only shift if beneficial.
        {
            return ("ShiftGrid", -1, -1, null, null, bestShiftX, bestShiftY);
        }

        // Return the chosen action and its parameters
        return chosenAction switch
        {
            "PlacePiece" => (chosenAction, bestMove.x, bestMove.y, null, null, null, null),
            "MovePiece" => (chosenAction, bestPieceMove.oldX, bestPieceMove.oldY, bestPieceMove.newX,
                bestPieceMove.newY, null, null),
            "ShiftGrid" => (chosenAction, -1, -1, null, null, bestGridShift.gridStartX,
                bestGridShift.gridStartY),
            _ => throw new InvalidOperationException("No valid action found!")
        };
    }

    private int EvaluateMove(int x, int y)
    {
        // Check if the move leads to a win
        if (CheckSelfWin())
        {
            return 1;
        }

        // Check if it blocks the opponent's win
        GameState.GameBoard[x][y] = GetOpponentPiece();
        bool opponentCanWin = CheckOpponentWin();
        GameState.GameBoard[x][y] = EGamePiece.Empty;

        if (opponentCanWin)
        {
            return 0;
        }

        return -1;
    }

    private void SimulateGridShift(int newStartX, int newStartY)
    {
        GameState.GridStartX = newStartX;
        GameState.GridStartY = newStartY;
        GameState.GridEndX = newStartX + GridX - 1;
        GameState.GridEndY = newStartY + GridY - 1;
    }

    private void UndoGridShift()
    {
        GameState.GridStartX = GridStartX;
        GameState.GridEndX = GridEndX;
        GameState.GridStartY = GridStartY;
        GameState.GridEndY = GridEndY;
    }

    private int EvaluateBoard()
    {
        // Count the number of enemy pieces inside the current grid.
        int enemyCount = 0;
        var opponent = GetOpponentPiece();

        for (int x = GameState.GridStartX; x <= GameState.GridEndX; x++)
        {
            for (int y = GameState.GridStartY; y <= GameState.GridEndY; y++)
            {
                if (GameState.GameBoard[x][y] == opponent)
                {
                    enemyCount++;
                }

            }
        }
        if (CountOpponentWinningMovesInsideGrid() > 0)
        {
            return -int.MaxValue;
        }
        // Lower enemyCount gives a higher score to this grid position.
        return -enemyCount;
    }
    
    private (int shiftX, int shiftY) FindBestGridShift()
    {
        int bestScore = int.MinValue;
        (int newGridStartX, int newGridStartY) bestShift = (0, 0);
        for (int x = 0; x <= DimX - GridX; x++)
        {
            for (int y = 0; y <= DimY - GridY; y++)
            {
                SimulateGridShift(x, y);
                int score = EvaluateBoard();
                UndoGridShift();
                if (score > bestScore)
                {
                    
                    var random = new Random();
                    var randomShiftX = random.Next(0, DimX - GridX);
                    var randomShiftY = random.Next(0, DimY - GridY);

                    bestShift = (randomShiftX, randomShiftY);

                    if (GameState.AiGridMoves.Count(v => v.GridStartX == x && v.GridStartY == y) < 2)
                    {
                        bestScore = score;
                        bestShift = (x, y);
                    }
                }

            }
        }

        return bestShift;
    }

    private (int oldX, int oldY, int newX, int newY, int bestScore, string choosenAction) 
        SimulateMovingPiece(int oldX, int oldY, int newX, int newY, int bestScore, string chosenAction,
            (int bestOldX, int bestOldY, int bestNewX, int bestNewY) bestPieceMove)
    {
        GameState.GameBoard[oldX][oldY] = GetOpponentPiece();
        bool opponentCanWin = CheckOpponentWin();

        GameState.GameBoard[oldX][oldY] = EGamePiece.Empty;
        GameState.GameBoard[newX][newY] = GameState.NextMoveBy;

        var moveScore = EvaluateMove(newX, newY);
        GameState.GameBoard[newX][newY] = EGamePiece.Empty;
        GameState.GameBoard[oldX][oldY] = GameState.NextMoveBy;

        if (moveScore > bestScore && !opponentCanWin)
        {
            bestScore = moveScore;
            bestPieceMove = (oldX, oldY, newX, newY);
            chosenAction = "MovePiece";
        }

        return (bestPieceMove.bestOldX, bestPieceMove.bestOldY, bestPieceMove.bestNewX, bestPieceMove.bestNewY, bestScore, chosenAction);
    }
    
    private EGamePiece GetOpponentPiece()
    {
        return GetNextMoveBy() == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }

    private bool CheckOpponentWin()
    {
        return WinColumn() == GetOpponentPiece() ||
               WinRow() == GetOpponentPiece() ||
               WinDiagonal() == GetOpponentPiece();
    }
    
    private int CountOpponentWinningMovesInsideGrid()
    {
        int winCount = 0;
        var opponent = GetOpponentPiece();

        for (int x = GameState.GridStartX; x <= GameState.GridEndX; x++)
        {
            for (int y = GameState.GridStartY; y <= GameState.GridEndY; y++)
            {
                // Only consider empty spots where the opponent can move.
                if (GameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    // Simulate the opponent placing a piece.
                    GameState.GameBoard[x][y] = opponent;
                    if (CheckOpponentWin())
                    {
                        winCount++;
                    }
                    // Undo the move.
                    GameState.GameBoard[x][y] = EGamePiece.Empty;

                    // If more than 1 winning move is found, no need to continue checking.
                    if (winCount > 1)
                    {
                        return winCount;
                    }
                }
            }
        }

        return winCount;
    }
    
    private int CountSelfWinningMovesInsideGrid()
    {
        int winCount = 0;
        var self = GameState.NextMoveBy;

        for (int x = GameState.GridStartX; x <= GameState.GridEndX; x++)
        {
            for (int y = GameState.GridStartY; y <= GameState.GridEndY; y++)
            {
                // Only consider empty spots where the opponent can move.
                if (GameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    // Simulate the opponent placing a piece.
                    GameState.GameBoard[x][y] = self;
                    if (CheckSelfWin())
                    {
                        winCount++;
                    }
                    // Undo the move.
                    GameState.GameBoard[x][y] = EGamePiece.Empty;

                    // If more than 1 winning move is found, no need to continue checking.
                    if (winCount > 1)
                    {
                        return winCount;
                    }
                }
            }
        }

        return winCount;
    }

    private bool CheckSelfWin()
    {
        return WinColumn() == GetNextMoveBy() ||
               WinRow() == GetNextMoveBy() ||
               WinDiagonal() == GetNextMoveBy();
    }

    private (int x, int y) GetRandomMove()
    {
        var random = new Random();
        var x = random.Next(0, DimX);
        var y = random.Next(0, DimY);

        while (!IsInsideGrid(x, y) || GameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            x = random.Next(0, DimX);
            y = random.Next(0, DimY);
        }

        return (x, y);
    }

    private (int x, int y) GetMoveNextToExistingOwnPiece()
    {
        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x < DimX; x++)
            {
                if (GameState.GameBoard[x][y] == GetNextMoveBy())
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        var nextX = x + i;
                        for (int j = -1; j <= 1; j++)
                        {
                            var nextY = y + j;
                            if (ValidateCoordinates(nextX, nextY) &&
                                GameState.GameBoard[nextX][nextY] == EGamePiece.Empty &&
                                IsInsideGrid(nextX, nextY))
                            {
                                return (nextX, nextY);
                            }
                        }
                    }
                }
            }
        }

        return (-1, -1);
    }
    
    private List<(int x, int y)> GetPiecesOutsideGrid(EGamePiece playerPiece)
    {
        var piecesOutside = new List<(int x, int y)>();

        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x < DimX; x++)
            {
                if (GameState.GameBoard[x][y] == playerPiece && !IsInsideGrid(x, y))
                {
                    piecesOutside.Add((x, y));
                }
            }
        }

        return piecesOutside;
    }

    private int GetPlayersPlacedPiecesAmount(EGamePiece player)
    {
        var amount = 0;
        for (int y = 0; y < DimY; y++)
        {
            for (int x = 0; x < DimX; x++)
            {
                if (GameState.GameBoard[x][y] == player)
                {
                    amount++;
                }
            }
        }

        return amount;
    }
}