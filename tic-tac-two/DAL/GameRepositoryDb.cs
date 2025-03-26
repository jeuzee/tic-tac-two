using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryDb(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Game>> GetUserGamesListById(int userId)
    {
        var games = await _ctx.Games
            .Include(g => g.Config)
            .Where(g => g.UserId == userId || g.UserId == null).ToListAsync();

        return games;
    }

    /**
     * Get game by id.
     */
    public async Task<Game?> LoadGame(int gameId)
    {
        return await _ctx.Games.Include(g => g.Config).FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public List<string> GetGameNames()
    {
        return _ctx.Games.Select(g => g.GameName).ToList();
    }

    public GameState GetGameByName(string name)
    {
        var game = _ctx.Games.AsNoTracking().SingleOrDefault(g => g.GameName == name);
        var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(game!.GameState);
        return gameState!;
    }
    
    public string GameNameToDisplay(string gameName)
    {
        return gameName;
    }

    public async Task<int> SaveGame(string jsonStateString, string gameConfigName, string gameName)
    {
        var existingGame = await _ctx.Games.FirstOrDefaultAsync(g => g.GameName == gameName);

        if (existingGame != null)
        {
            existingGame.GameState = jsonStateString;
            existingGame.GameName = gameConfigName + " " + DateTime.Now.ToString("O").Replace(":", "-");
            existingGame.ModifiedAt = DateTime.Now;
        }
        else
        {
            var config = await _ctx.Configs.SingleAsync(c => c.ConfigName == gameConfigName);
            var dbGame = new Game
            {
                GameName = gameConfigName + " " + DateTime.Now.ToString("O").Replace(":", "-"),
                GameState = jsonStateString,
                ConfigId = config.Id
            };
            await _ctx.AddAsync(dbGame);
            existingGame = dbGame;
        }
        
        await _ctx.SaveChangesAsync();
        return existingGame.Id;
    }

    public async Task<int> AddGame(Game game)
    {
        game.GameName = game.GameName;
        _ctx.Add(game);
        await _ctx.SaveChangesAsync();
        return game.Id;
    }

    public void DeleteGame(string gameNameToDelete)
    {
        var gameToDelete = _ctx.Games.SingleOrDefault(g => g.GameName == gameNameToDelete);
        if (gameToDelete != null)
        {
            _ctx.Games.Remove(gameToDelete);
            _ctx.SaveChanges(); 
        }
    }
}