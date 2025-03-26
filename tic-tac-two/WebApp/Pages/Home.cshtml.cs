using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class Home : PageModel
{
	public string UserName { get; set; } = default!;
	public int UserId { get; set; } = default!;

	private readonly IConfigRepository _configRepository;
	private readonly IGameRepository _gameRepository;
	private readonly IUserRepository _userRepository;

	public SelectList ConfigSelectList { get; set; } = default!;
	public SelectList GameModes { get; set; } = default!;

	[BindProperty]
	public int ConfigId { get; set; }
	
	[BindProperty]
	public EGameMode GameMode { get; set; }

	public Home(IConfigRepository configRepository, IGameRepository gameRepository, IUserRepository userRepository)
	{
		_configRepository = configRepository;
		_gameRepository = gameRepository;
		_userRepository = userRepository;
	}

	public IActionResult OnGet()
	{
		UserName = HttpContext.Session.GetString("UserName") ?? "";
		if (string.IsNullOrEmpty(UserName))
		{
			return RedirectToPage("./Index", new { error = "No username provided." });
		}

		
		var user = _userRepository.GetUserByName(UserName).Result;
		if (user == null)
		{				
			user = _userRepository.AddUser(UserName).Result;
		}

		HttpContext.Session.SetInt32("UserId", user.Id);

		UserId = HttpContext.Session.GetInt32("UserId") ?? -1;

		var usrCfgs = _configRepository.GetUserSelectConfigItemListById(UserId).Result;

		ConfigSelectList = new SelectList(usrCfgs, nameof(ConfigSelectItem.Id), nameof(ConfigSelectItem.ConfigName));

		GameModes = new SelectList(Enum.GetValues(typeof(EGameMode)).Cast<EGameMode>());
		return Page();
	}

	public IActionResult OnPost()
	{
		UserName = HttpContext.Session.GetString("UserName") ?? "";
		UserId = HttpContext.Session.GetInt32("UserId") ?? -1;

		var selectedConfigId = int.Parse(Request.Form["Config"]!);
		var selectedGameMode = Enum.Parse<EGameMode>(Request.Form["GameMode"]!);
		
		var cfg = _configRepository.GetConfigurationById(selectedConfigId);

		var gameInstance = new TicTacTwoBrain(cfg);

		var newGame = new Game
		{
			GameName = gameInstance.GetGameConfigName(),
			GameState = gameInstance.SetGameStateJson(),
			ConfigId = selectedConfigId,
			UserId = UserId,
		};
		
		var newGameId = _gameRepository.AddGame(newGame).Result;

		return RedirectToPage("./PlayGame", new { gameId = newGameId, gameMode = selectedGameMode });
	}
}