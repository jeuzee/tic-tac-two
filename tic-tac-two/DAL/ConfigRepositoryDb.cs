using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly AppDbContext _ctx;

    public ConfigRepositoryDb(AppDbContext ctx)
    {
        _ctx = ctx;
        CheckAndCreateInitialConfig();
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!_ctx.Configs.Any())
        {
            var basicConfigs = new List<Config>()
            {
                new Config
                {
                    ConfigName = "Classical Tic-Tac-Toe",
                    BoardWidth = 3,
                    BoardHeight = 3,
                    WinCondition = 3,
                    AmountOfPieces = 5,
                    MovePieceAndGridAfterNMoves = 0,
                    GridWidth = 0,
                    GridHeight = 0,
                },
                new Config
                {
                    ConfigName = "Classical Tic-Tac-Two",
                    BoardWidth = 5,
                    BoardHeight = 5,
                    WinCondition = 3,
                    AmountOfPieces = 4,
                    MovePieceAndGridAfterNMoves = 4,
                    GridWidth = 3,
                    GridHeight = 3,
                }
            };
            
            _ctx.Configs.AddRange(basicConfigs);
            _ctx.SaveChanges();
        }
    }
    
    public async Task<List<Config>> GetUserConfigsListById(int userId)
    {
        var configs = await _ctx.Configs.Where(c => c.UserId == userId || c.UserId == null).ToListAsync();
        return configs;
    }

    /**
     * Return list of ConfigSelectItem for SelectList.
     */
    public async Task<List<ConfigSelectItem>> GetUserSelectConfigItemListById(int userId)
    {
        var configs = await _ctx.Configs
            .Where(c => c.UserId == userId || c.UserId == null)
            .Select(cfg => new ConfigSelectItem(cfg.Id, cfg.ConfigName))
            .ToListAsync();
        return configs;
    }
    
    public List<string> GetConfigurationNames()
    {
        return _ctx.Configs.Select(c => c.ConfigName).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var config = _ctx.Configs.AsNoTracking().SingleOrDefault(c => c.ConfigName == name);
        var cfg = ConfigMapper.ConfigToGameConfiguration(config!);
        
        return cfg;
    }

    public string ConfigNameToDisplay(string gameName)
    {
        return gameName;
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        var config = _ctx.Configs.AsNoTracking().FirstOrDefault(c => c.Id == id);

        var cfg = ConfigMapper.ConfigToGameConfiguration(config!);
        
        return cfg;
    }

    public async Task<Config?> GetConfigById(int configId)
    {
        var config = await _ctx.Configs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == configId);

        return config;
    }

    public async Task<string> SaveConfig(GameConfiguration cfg, string name)
    {
        var existingCfg = await _ctx.Configs.FirstOrDefaultAsync(c => c.ConfigName == name);
        if (existingCfg != null)
        {
            existingCfg.ConfigName = cfg.Name;
            existingCfg.BoardWidth = cfg.BoardWidth;
            existingCfg.BoardHeight = cfg.BoardHeight;
            existingCfg.GridWidth = cfg.GridWidth;
            existingCfg.GridHeight = cfg.GridHeight;
            existingCfg.WinCondition = cfg.WinCondition;
            existingCfg.AmountOfPieces = cfg.AmountOfPieces;
            existingCfg.MovePieceAndGridAfterNMoves = cfg.MovePieceAndGridAfterNMoves;   
            existingCfg.ModifiedAt = DateTime.Now;
        }
        else
        {
            var dbConfig = ConfigMapper.GameConfigurationToConfig(cfg);
            
            existingCfg = dbConfig;
            await _ctx.Configs.AddAsync(dbConfig);
        }
        await _ctx.SaveChangesAsync();
        return existingCfg.ConfigName;
    }

    public async Task AddConfig(Config config)
    {
        var uniqueName = CreateNonExistingCfgName(config.ConfigName);
        config.ConfigName = uniqueName;
        _ctx.Add(config);
        await _ctx.SaveChangesAsync();
    }

    public void DeleteConfig(string nameOfConfigToDelete)
    {
        var configToDelete = _ctx.Configs.SingleOrDefault(c => c.ConfigName == nameOfConfigToDelete);
        if (configToDelete != null)
        {
            _ctx.Configs.Remove(configToDelete);
            _ctx.SaveChanges();
        }
    }

    public string CreateNonExistingCfgName(string name)
    {
        var toReturn = name;
        var num = 1;
        while (_ctx.Configs.Any(c => c.ConfigName == toReturn))
        {
            toReturn = name + num;
            num++;
        }
        return toReturn;
    }
}