using Domain;
using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    public List<string> GetConfigurationNames();
    
    public GameConfiguration GetConfigurationByName(string name);
    
    public string ConfigNameToDisplay(string gameName);

    public Task<string> SaveConfig(GameConfiguration configuration, string name);
    
    public Task AddConfig(Config config);
    
    public void DeleteConfig(string nameOfConfigToDelete);

    public string CreateNonExistingCfgName(string name);

    public Task<List<Config>> GetUserConfigsListById(int userId);
    
    public Task<List<ConfigSelectItem>> GetUserSelectConfigItemListById(int userId);
    
    public GameConfiguration GetConfigurationById(int id);
    
    public Task<Config?> GetConfigById(int configId);
}