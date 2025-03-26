using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Config: BaseEntity
{
    [MaxLength(128, ErrorMessage = "Config Name cannot be longer than 128 characters.")]
    [Required(ErrorMessage = "Config Name is required.")]
    public string ConfigName { get; set; } = default!;
    
    [Range(3, 20, ErrorMessage = "Board Width must be between 3 and 20.")]
    [Required(ErrorMessage = "Board Width is required.")]
    public int BoardWidth { get; set; }
    
    [Range(3, 20, ErrorMessage = "Board Height must be between 3 and 20.")]
    [Required(ErrorMessage = "Board Height is required.")]
    public int BoardHeight { get; set; }

    [Range(3, int.MaxValue, ErrorMessage = "Win Condition must be at least 3.")]
    [Required(ErrorMessage = "Win Condition is required.")]
    public int WinCondition { get; set; }
    
    [Range(3, int.MaxValue, ErrorMessage = "Amount of Pieces must be at least 3.")]
    [Required(ErrorMessage = "Amount of Pieces is required.")]
    public int AmountOfPieces { get; set; }
    
    [Range(2, int.MaxValue, ErrorMessage = "Move Piece and Grid After N Moves must be at least 2.")]
    [Required(ErrorMessage = "Move Piece and Grid After N Moves is required.")]
    public int MovePieceAndGridAfterNMoves { get; set; }

    [Range(0, 20, ErrorMessage = "Grid Width must be between 0 and 20.")]
    [Required(ErrorMessage = "Grid Width is required.")]
    public int GridWidth { get; set; }
    
    [Range(0, 20, ErrorMessage = "Grid Height must be between 0 and 20.")]
    [Required(ErrorMessage = "Grid Height is required.")]
    public int GridHeight { get; set; }
    
    public ICollection<Game>? Games { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public override string ToString()
    {
        return Id + " " + ConfigName + "(" + BoardWidth + "x" + BoardHeight + ") Games: " + (Games?.Count.ToString() ?? "not joined");
    }
    
    public bool IsValidWinCondition()
    {
        // WinCondition should not exceed GridWidth or GridHeight
        if (((WinCondition > GridWidth || WinCondition > GridHeight) && (GridWidth != 0 && GridHeight != 0)) ||
            WinCondition > BoardWidth || WinCondition > BoardHeight ||
            WinCondition > AmountOfPieces)
            return false;

        return true;
    }

    public bool IsGridValid()
    {
        // Allow grid to be 0x0 only if both GridWidth and GridHeight are 0
        if (GridWidth == 0 && GridHeight == 0)
            return true;

        // GridWidth and GridHeight should not exceed BoardWidth and BoardHeight
        if (GridWidth <= BoardWidth && GridHeight <= BoardHeight && GridWidth != 0 && GridHeight != 0)
            return true;
    
        return false;
    }
}