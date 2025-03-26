using System.Text.Json;
using Domain;

namespace DAL;

public class UserRepositoryJson: IUserRepository
{
    private List<User> _users = new();
    private int _currentMaxId;

    public UserRepositoryJson()
    {
        LoadUsers();
        LoadCurrentMaxId();
    }

    // Load users from the JSON file
    private void LoadUsers()
    {
        if (File.Exists(FileHelper.BasePath + FileHelper.UsersJson))
        {
            var usersJson = File.ReadAllText(FileHelper.BasePath + FileHelper.UsersJson);
            _users = JsonSerializer.Deserialize<List<User>>(usersJson) ?? new List<User>();
        }
        else
        {
            File.WriteAllText(FileHelper.BasePath + FileHelper.UsersJson, "[]");
        }
    }

    // Save users to the JSON file
    private void SaveUsers()
    {
        var usersJson = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileHelper.BasePath + FileHelper.UsersJson, usersJson);
    }

    private void LoadCurrentMaxId()
    {
        _currentMaxId = _users.Count != 0 ? _users.Max(u => u.Id) : 0;
        _currentMaxId++;
    }

    public Task<User?> GetUserByName(string userName)
    {
        var user = _users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public async Task<User> AddUser(string userName)
    {
        var existingUser = await GetUserByName(userName);
        if (existingUser != null)
        {
            return existingUser;
        }

        var user = new User
        {
            Id = _currentMaxId,
            UserName = userName,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };

        _users.Add(user);
        SaveUsers();

        return user;
    }
}