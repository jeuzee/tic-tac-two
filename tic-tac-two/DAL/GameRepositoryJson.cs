using System.Globalization;
using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryJson: IGameRepository
{
    private int _currentMaxId;
    private Lazy<IConfigRepository> _configRepository;
    
    public GameRepositoryJson()
    {
        _configRepository = new Lazy<IConfigRepository>(() => new ConfigRepositoryJson());
        LoadCurrentMaxId();
    }

    private void LoadCurrentMaxId()
    {
        var existingFiles = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension);

        foreach (var file in existingFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split("@");
            if (parts.Length > 1 && int.TryParse(parts[0], out var id))
            {
                _currentMaxId = Math.Max(_currentMaxId, id);
            }
        }
        _currentMaxId++;
    }
    
    /**
     * Get game by id.
     */
    public Task<Game?> LoadGame(int gameId)
    {
        var listOfGameNames = GetGameNames();
        var game = new Game();
        foreach (var name in listOfGameNames)
        {
            if (name.StartsWith(gameId.ToString()))
            {
                game = CreateGameObject(name);
            }
        }
        return Task.FromResult(game)!;
    }

    private Game CreateGameObject(string gameName)
    {
        const string format = "yyyy:MM:ddTHH:mm:ss.fffffffK";
        var gameState = GetGameByName(gameName);
        var gameConfigName = gameState.GameConfiguration.Name;
        var splitted = gameName.Split("@");

        var game = new Game
        {
            GameName = gameName,
            GameState = gameState.ToString(),
            ConfigId = int.Parse(gameConfigName.Split("@")[0]),
            Id = int.Parse(splitted[0]),
            CreatedAt = DateTimeOffset.ParseExact(splitted[3].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime,
            ModifiedAt = DateTimeOffset.ParseExact(splitted[4].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime
        };
        var cfg = _configRepository.Value.GetConfigById(game.ConfigId).Result;
        
        cfg!.ConfigName = cfg.ConfigName.Split("@")[1];
        game.Config = cfg;

        return game;
    }
    
    public Task<List<Game>> GetUserGamesListById(int userId)
    {
        var allGameNames = GetGameNames();
        var games = new List<Game>();
        foreach (var name in allGameNames)
        {
            var usrId = name.Split("@")[2];
            if (string.Equals(usrId, userId.ToString()) || string.Equals(usrId, "null"))
            {
                var game = CreateGameObject(name);
                game.GameName = name.Split("@")[1] + " " + name.Split("@")[3];
                games.Add(game);
            }
        }
        
        return Task.FromResult(games);
    }

    public async Task<int> AddGame(Game game)
    {
        game.GameName = GenerateFileName(game.GameName, game.UserId, _currentMaxId, game.ModifiedAt.ToString("O").Replace(":", "-"));
        var jsonStateStr = game.GameState.ToString();
        var fileName = FileHelper.BasePath + game.GameName + FileHelper.GameExtension;

        await File.WriteAllTextAsync(fileName, jsonStateStr);
        var id = int.Parse(game.GameName.Split("@")[0]);

        return id;
    }


    public async Task<int> SaveGame(string jsonStateString, string gameConfigName, string gameStateName)
    {
        var gameId = _currentMaxId;
        int? userId = null;
        string? createdAt = null;
        if (File.Exists(FileHelper.BasePath + gameStateName + FileHelper.GameExtension))
        {
            DeleteGame(gameStateName);
            if (int.TryParse(gameStateName.Split("@")[0], out var id))
            {
                gameId = id;
            }
            if (int.TryParse(gameStateName.Split("@")[2], out var usrId))
            {
                userId = usrId;
            }

            createdAt = gameStateName.Split("@")[3];
        }
        var gameName = GenerateFileName(gameConfigName, userId, gameId, createdAt);
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;

        await File.WriteAllTextAsync(fileName, jsonStateString);
        return gameId;
    }

    public List<string> GetGameNames()
    {
        var res = new List<string>();
        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension))
        {
            var filenameParts = Path.GetFileNameWithoutExtension(fullFileName);
            var primaryName = Path.GetFileNameWithoutExtension(filenameParts);
            res.Add(primaryName);
        }
        
        return res;
    }

    public GameState GetGameByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        var gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(configJsonStr);
        return gameState!;
    }

    public string GameNameToDisplay(string gameName)
    {
        var name = gameName.Split("@")[1];
        var modifiedAt = gameName.Split("@")[4];
        return $"{name} {modifiedAt}";
    }
    
    public void DeleteGame(string nameOfGameToDelete)
    {
        var gamePath = FileHelper.BasePath + nameOfGameToDelete + FileHelper.GameExtension;
        File.Delete(gamePath);
    }

    private string GenerateFileName(string fullCfgName, int? userId, int? gameId, string? createdAt)
    {
        var cfgNameParts = fullCfgName.Split("@");
        var cfgName = cfgNameParts[1];
        var modifiedAt = DateTime.Now.ToString("O").Replace(":", "-");
        
        if (createdAt == null)
        {
            createdAt = DateTime.Now.ToString("O").Replace(":", "-");
        }
        
        var fileName = $"{gameId}@{cfgName}@null@{createdAt}@{modifiedAt}";

        if (userId != null)
        {
            fileName = $"{gameId}@{cfgName}@{userId}@{createdAt}@{modifiedAt}";
        }

        if (gameId == _currentMaxId) _currentMaxId++;
        return fileName;
    }
}