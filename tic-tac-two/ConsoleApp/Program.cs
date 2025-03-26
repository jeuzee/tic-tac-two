using ConsoleApp;
using DAL;


var configRepository = new ConfigRepositoryJson();
var gameRepository = new GameRepositoryJson(); 
    
// var dbContext = new AppDbContextFactory().CreateDbContext([]);
//
// var configRepository = new ConfigRepositoryDb(dbContext);
// var gameRepository = new GameRepositoryDb(dbContext); 

Menus.Init(configRepository, gameRepository);

Menus.MainMenu.Run();
