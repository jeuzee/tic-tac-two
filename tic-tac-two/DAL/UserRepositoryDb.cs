using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class UserRepositoryDb: IUserRepository
{
    private readonly AppDbContext _ctx;

    public UserRepositoryDb(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<User?> GetUserByName(string userName)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        return user;
    }

    public async Task<User> AddUser(string userName)
    {
        var user = new User { UserName = userName };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
    
        return user;
    }
}