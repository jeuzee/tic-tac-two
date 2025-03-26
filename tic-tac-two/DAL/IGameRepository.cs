using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public List<string> GetGameNames();
    
    public GameState GetGameByName(string name);

    public string GameNameToDisplay(string gameName);
    
    public Task<int> SaveGame(string jsonStateString, string gameConfigName, string gameName);

    public Task<int> AddGame(Game game);
    
    public void DeleteGame(string gameNameToDelete);

    public Task<Game?> LoadGame(int gameId);

    public Task<List<Game>> GetUserGamesListById(int userId);
}