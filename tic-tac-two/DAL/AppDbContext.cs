using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Config> Configs { get; set; } = default!;
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {   
    }
}