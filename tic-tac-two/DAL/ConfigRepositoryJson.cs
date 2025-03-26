using System.Data.Common;
using System.Globalization;
using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryJson: IConfigRepository
{
    private int _currentMaxId = 2;
    private Lazy<IGameRepository> _gameRepository;
    
    public ConfigRepositoryJson()
    {
        LoadCurrentMaxId();
        
        _gameRepository = new Lazy<IGameRepository>(() => new GameRepositoryJson());

    }

    private void LoadCurrentMaxId()
    {
        var existingFiles = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension);
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

    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();
        var res = new List<string>();
        foreach (var fullFileName in Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension))
        {
            var filenameParts = Path.GetFileNameWithoutExtension(fullFileName);
            var primaryName = Path.GetFileNameWithoutExtension(filenameParts);
            res.Add(primaryName);
        }

        return res;
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
        return config;
    }

    public string ConfigNameToDisplay(string gameName)
    {
        var splitted = gameName.Split("@");
        return $"{splitted[1]}";
    }

    public Task<List<Config>> GetUserConfigsListById(int userId)
    {
        var allConfigNames = GetConfigurationNames();
        var configs = new List<Config>();
        var format = "yyyy:MM:ddTHH:mm:ss.fffffffK";

        foreach (var name in allConfigNames)
        {
            var id = name.Split("@")[2];
            if (string.Equals(id, userId.ToString()) || string.Equals(id, "null"))
            {
                var cfg = GetConfigurationByName(name);
                var config = ConfigMapper.GameConfigurationToConfig(cfg);
                var splitted = name.Split("@");

                config.ConfigName = splitted[1];
                config.Id = int.Parse(splitted[0]);
                config.CreatedAt = DateTimeOffset
                    .ParseExact(splitted[3].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime;
                config.ModifiedAt = DateTimeOffset
                    .ParseExact(splitted[4].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime;
                configs.Add(config);
            }
        }

        return Task.FromResult(configs);
    }

    public Task<List<ConfigSelectItem>> GetUserSelectConfigItemListById(int userId)
    {
        var configSelectItems = new List<ConfigSelectItem>();

        var userConfigs = GetUserConfigsListById(userId).Result;
        foreach (var cfg in userConfigs)
        {
            configSelectItems.Add(new ConfigSelectItem(cfg.Id, cfg.ConfigName));
        }


        return Task.FromResult(configSelectItems);
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        var allConfigNames = GetConfigurationNames();
        var config = new GameConfiguration();
        foreach (var name in allConfigNames)
        {
            if (string.Equals(name.Split("@")[0], id.ToString()))
            {
                config = GetConfigurationByName(name);
            }
        }
        return config;
    }

    public Task<Config?> GetConfigById(int configId)
    {
        var format = "yyyy:MM:ddTHH:mm:ss.fffffffK";
        var cfg = GetConfigurationById(configId);

        if (string.IsNullOrEmpty(cfg.Name))
        {
            return Task.FromResult<Config?>(null);
        }
        
        var config = ConfigMapper.GameConfigurationToConfig(cfg);

        var splitted = cfg.Name.Split("@");
        config.Id = configId;
        config.CreatedAt = DateTimeOffset
            .ParseExact(splitted[3].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime;
        config.ModifiedAt = DateTimeOffset
            .ParseExact(splitted[4].Replace("-", ":"), format, CultureInfo.InvariantCulture).DateTime;

        return Task.FromResult(config)!;
    }

    private static void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
        
        var data = Directory.GetFiles(FileHelper.BasePath,"*" + FileHelper.ConfigExtension).ToList();
        
         if (data.Count == 0)
         {
             var basicConfigurations = new List<GameConfiguration>()
             {
                 new GameConfiguration()
                 {
                     Name = $"1@Classical Tic-Tac-Toe" +
                            $"@null" +
                            $"@{DateTime.Now.ToString("O").Replace(":", "-")}" +
                            $"@{DateTime.Now.ToString("O").Replace(":", "-")}"
                 },
                 new GameConfiguration()
                 {
                     Name = $"2@Classical Tic-Tac-Two" +
                            $"@null" +
                            $"@{DateTime.Now.ToString("O").Replace(":", "-")}" +
                            $"@{DateTime.Now.ToString("O").Replace(":", "-")}",
                     BoardWidth = 5,
                     BoardHeight = 5,
                     WinCondition = 3,
                     AmountOfPieces = 4,
                     MovePieceAndGridAfterNMoves = 4,
                     GridWidth = 3,
                     GridHeight = 3,
                 },
             };
            foreach (var config in basicConfigurations)
            {
                var optionJsonStr = System.Text.Json.JsonSerializer.Serialize(config);
                File.WriteAllText(FileHelper.BasePath + config.Name + FileHelper.ConfigExtension, optionJsonStr);
            }
        }
    }
        
    public async Task<string> SaveConfig(GameConfiguration configuration, string name)
    {   
        var cfgId = _currentMaxId;
        var cfgName = configuration.Name;
        int? userId = null;
        string? createdAt = null;

        var splitted = name.Split("@");


        if (File.Exists(FileHelper.BasePath + name + FileHelper.ConfigExtension))
        {
            DeleteOnlyConfig(name);
            if (cfgName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                cfgName = splitted[1];
            }

            if (int.TryParse(splitted[0], out var configId))
            {
                cfgId = configId;
            }
            
            if (int.TryParse(splitted[2], out var usrId))
            {
                userId = usrId;
            }

            createdAt = splitted[3];
        }
        configuration.Name = GenerateFileName(cfgName, userId, cfgId, createdAt);
        // Serialize and save the configuration
        var configJson = System.Text.Json.JsonSerializer.Serialize(configuration);
        var configPath = FileHelper.BasePath + configuration.Name + FileHelper.ConfigExtension;
        
        await File.WriteAllTextAsync(configPath, configJson);
        return configuration.Name;
    }

    public async Task AddConfig(Config config)
    {
        config.ConfigName = GenerateFileName(
            config.ConfigName,
            config.UserId,
            _currentMaxId,
            config.CreatedAt.ToString("O").Replace(":", "-")
        );

        var gameConfiguration = ConfigMapper.ConfigToGameConfiguration(config);
        
        var configJson = System.Text.Json.JsonSerializer.Serialize(gameConfiguration);
        var configPath = FileHelper.BasePath + gameConfiguration.Name + FileHelper.ConfigExtension;
        await File.WriteAllTextAsync(configPath, configJson);
    }
    
    /**
     * Delete only config.
     * Used on cfg updates.
     */
    private static void DeleteOnlyConfig(string nameOfConfigToDelete)
    {
        var configPath = FileHelper.BasePath + nameOfConfigToDelete + FileHelper.ConfigExtension;
        File.Delete(configPath);
    }

    /**
     * Delete config with all its games.
     * Used on cfg delete.
     */
    public void DeleteConfig(string nameOfCfg)
    {
        DeleteOnlyConfig(nameOfCfg);
        var configId = nameOfCfg.Split("@")[0];
        var gameNames = _gameRepository.Value.GetGameNames();
        foreach (var name in gameNames)
        {
            var game = _gameRepository.Value.GetGameByName(name);
            if (game.GameConfiguration.Name.Split("@")[0].Equals(configId))
            {
                _gameRepository.Value.DeleteGame(name);
            }
        }
    }

    public string CreateNonExistingCfgName(string name)
    {
        var num = 1;
        var existingConfigNames = GetConfigurationNames();
        var toReturn = name;
        foreach (var cfg in existingConfigNames)
        {
            var cfgParts = cfg.Split("@");
            var cfgName = cfgParts[1];
            if (string.Equals(toReturn.ToLower(), cfgName.ToLower()))
            {
                toReturn = name + num;
                num++;
            }
        }
       
        return toReturn;
    }
    
    private string GenerateFileName(string cfgName, int? userId, int cfgId, string? createdAt)
    {
        cfgName = CreateNonExistingCfgName(cfgName);
        var modifiedAt = DateTime.Now.ToString("O").Replace(":", "-");

        if (createdAt == null)
        {
            createdAt = DateTime.Now.ToString("O").Replace(":", "-");
        }
        
        var fileName = $"{cfgId}@{cfgName}@null@{createdAt}@{modifiedAt}";

        if (userId != null)
        {
            fileName = $"{cfgId}@{cfgName}@{userId}@{createdAt}@{modifiedAt}";
        }

        if (cfgId == _currentMaxId) _currentMaxId++;
        
        return fileName;
    }
}