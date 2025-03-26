# TIC-TAC-TWO in Console and Web

### Introduction
- Game was written as a course project in TalTech in Autumn 2024 <br>
- Can be played from Console and Web <br>
- Web is done via `Razor Pages` <br>
- It supports `custom configs` that can be added/edited in both Console and Web UI <br>
- App supports both `JSON` and `Database` ways of storaging users configurations and games. <br>
- `JSON` and `Database` are separate, so swapping from one to another doesn't transfer data between them <br>
- Game has a relatively smart AI and three different game modes: <br>
`Player vs Player` <br>
`Player vs AI` <br>
`AI vs AI` <br>

- To swap from `JSON` to `Database` or vice versa all that you need to do is change comment and uncomment corresponding lines in `program.cs` file <br>
For ConsoleApp `program.cs`: <br>
`var configRepository = new ConfigRepositoryJson();` <br>
`var gameRepository = new GameRepositoryJson();` <br>
`// var dbContext = new AppDbContextFactory().CreateDbContext([]);` <br>
`//` <br>
`// var configRepository = new ConfigRepositoryDb(dbContext);` <br>
`// var gameRepository = new GameRepositoryDb(dbContext);` <br>
For WebApp `program.cs`: <br>
`builder.Services.AddScoped<IConfigRepository, ConfigRepositoryJson>();` <br>
`builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();` <br>
`builder.Services.AddScoped<IUserRepository, UserRepositoryJson>();` <br>
`// builder.Services.AddScoped<IConfigRepository, ConfigRepositoryDb>();` <br>
`// builder.Services.AddScoped<IGameRepository, GameRepositoryDb>();` <br>
`// builder.Services.AddScoped<IUserRepository, UserRepositoryDb>();` <br>


