using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game: BaseEntity
{
    [MaxLength(128)]
    public string GameName { get; set; } = default!;
    
    [MaxLength(10240)]
    public string GameState { get; set; } = default!;
    
    // Expose the Foreign Key
    public int ConfigId { get; set; }
    public Config? Config { get; set; }
    
    public int? UserId { get; set; }
    public User? User { get; set; }
}