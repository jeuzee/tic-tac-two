using Domain;

namespace DAL;

public interface IUserRepository
{
    public Task<User?> GetUserByName(string userName);

    public Task<User> AddUser(string userName);
}