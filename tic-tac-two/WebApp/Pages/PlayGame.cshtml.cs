using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PlayGame : PageModel
{
    private readonly IGameRepository _gameRepository;

    public PlayGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public string UserName { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int GameId { get; set; }

    public TicTacTwoBrain GameEngine { get; set; } = default!;

    public EGamePiece Winner { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Mode { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public bool Reset { get; set; } = false;

    [BindProperty(SupportsGet = true)]
    public int OldX { get; set; } = -1;

    [BindProperty(SupportsGet = true)]
    public int OldY { get; set; } = -1;

    [BindProperty(SupportsGet = true)]
    public EGameMode GameMode { get; set; }

    public bool MoveDone = false;
    
    [BindProperty(SupportsGet = true)]
    public bool AiMove { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Warning { get; set; }
    
    public async Task<IActionResult> OnGet(int? x, int? y, int? newX, int? newY)
    {
        UserName = HttpContext.Session.GetString("UserName") ?? "";
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }

        var game = _gameRepository.LoadGame(GameId).Result;
        if (game == null)
        {
            return RedirectToPage("./Home", new { userName = UserName });
        }

        var gameState = _gameRepository.GetGameByName(game.GameName);

        GameEngine = new TicTacTwoBrain(gameState);

        GameEngine.SetGameStateJson();

        if (string.IsNullOrEmpty(GameEngine.GetPlayerX()))
        {
            GameEngine.SetPlayerX(UserName);
        } else if (string.IsNullOrEmpty(GameEngine.GetPlayerO()) && GameEngine.GetPlayerX() != UserName)
        {
            GameEngine.SetPlayerO(UserName);
        }
        Console.WriteLine(GameEngine.GetPlayerX());
        Console.WriteLine(GameEngine.GetPlayerO());
        Console.WriteLine(UserName);
        if (Reset)
        {
            GameEngine.ResetGame();
            GameId = await _gameRepository.SaveGame(
                GameEngine.SetGameStateJson(),
                GameEngine.GetGameConfigName(),
                game.GameName);
            Reset = false;
            
            return RedirectToPage("./PlayGame", new { gameId = GameId, gameMode = GameMode });
        }
        
        if ((x != null && y != null || (newX != null && newY != null)) &&
            (GameMode == EGameMode.PlayerVsPlayer || (GameMode == EGameMode.PlayerVsAi && GameEngine.GetNextMoveBy() == EGamePiece.X)))
        {
            if (GameMode == EGameMode.PlayerVsPlayer && GameEngine.GetNextMoveBy() == EGamePiece.X &&
                GameEngine.GetPlayerX() != UserName)
            {
                Warning = "Not ur turn!";
                return Page();
            }
            
            if (GameMode == EGameMode.PlayerVsPlayer && GameEngine.GetNextMoveBy() == EGamePiece.O &&
                    GameEngine.GetPlayerO() != UserName && !string.IsNullOrEmpty(GameEngine.GetPlayerO()))
            {
                Warning = "Not ur turn!";
                return Page();
            }

            if (Mode <= 1 && x != null && y != null)
            {
                GameEngine.MakeAMove(x.Value, y.Value);
            }
            else if (Mode == 2 && x != null && y != null)
            {
                GameEngine.MoveGrid(x.Value, y.Value);
            }
            else if (newX != null && newY != null)
            {
                GameEngine.MovePiece(OldX, OldY, newX.Value, newY.Value);
                OldX = -1;
                OldY = -1;
            }
            
            MoveDone = true;
        }
        
        if (GameEngine.WinColumn() == EGamePiece.X ||
            GameEngine.WinRow() == EGamePiece.X ||
            GameEngine.WinDiagonal() == EGamePiece.X)
        {
            Winner = EGamePiece.X;
        }

        if (GameEngine.WinColumn() == EGamePiece.O ||
            GameEngine.WinRow() == EGamePiece.O ||
            GameEngine.WinDiagonal() == EGamePiece.O)
        {
            Winner = EGamePiece.O;
        }
        
        if (((GameMode == EGameMode.PlayerVsAi && GameEngine.GetNextMoveBy() == EGamePiece.O) || (GameMode == EGameMode.AiVsAi && AiMove)) && Winner == EGamePiece.Empty)
        {
            GameEngine.AiAction();
            MoveDone = true;
        }

        if (MoveDone)
        {
            GameId = await _gameRepository.SaveGame(
                GameEngine.SetGameStateJson(),
                GameEngine.GetGameConfigName(),
                game.GameName);
            MoveDone = false;
            AiMove = false;
            return RedirectToPage("./PlayGame", new { gameId = GameId, gameMode = GameMode });
        }
        return Page();
    }
}