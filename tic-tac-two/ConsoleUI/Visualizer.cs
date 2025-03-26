using GameBrain;

namespace ConsoleUI;

public class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            /*
             * Draw outer border with x/y positions
             */
            if (y == 0)
            {
                Console.Write("x/y" + " | ");
                for (var x = 0; x < gameInstance.DimX; x++)
                {
                    Console.Write(x + " | ");
                }
                Console.WriteLine();
                Console.Write("----|");
                for (var x = 0; x < gameInstance.DimX; x++)
                {
                    Console.Write("---");
                    if (x != gameInstance.DimX - 1)
                    {
                        Console.Write("-");
                    }
                    else
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
            }
            Console.Write(" " + y + "  |");
            for (var x = 0; x < gameInstance.DimX; x++)
            {

                Console.ResetColor();
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
                if (x == gameInstance.DimX - 1) continue;
                if (x >= gameInstance.GridStartX && x < gameInstance.GridEndX && y >= gameInstance.GridStartY && y <=  gameInstance.GridEndY)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
                else
                {
                    Console.ResetColor();
                }
                Console.Write("|");
            }
            
            Console.Write("|"); // outer border
            Console.WriteLine();
            Console.Write("----|"); // outer border after Y pos
            
            /*
             * Draw border between place for pieces and draw bottom border.
             */
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if (x >= gameInstance.GridStartX && x <= gameInstance.GridEndX && y >= gameInstance.GridStartY && y < gameInstance.GridEndY)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
                else
                {
                    Console.ResetColor();
                }
                Console.Write("---");
                if (x != gameInstance.DimX - 1)
                {
                    // if it's not yet last Y, then draw + between, else draw another -
                    if (y != gameInstance.DimY - 1)
                    {
                        if (x == gameInstance.GridEndX)
                        {
                            Console.ResetColor();
                        }
                        Console.Write("+");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
                else // draw | at the end of each line
                {
                    Console.ResetColor();
                    Console.Write("|");
                }
            }
            Console.WriteLine();
        }
    }
    
    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };

}