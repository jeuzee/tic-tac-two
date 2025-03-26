using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Games
{
    public class DeleteModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public DeleteModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;
        
        public string UserName { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "";
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("../Index", new { error = "No username provided." });
            }

            if (!id.HasValue)
            {
                return NotFound();
            }
            
            var game = _gameRepository.LoadGame(id.Value).Result;

            if (game == null)
            {
                return NotFound();
            }
            else
            {
                Game = game;
                if (_gameRepository.GetType() == typeof(GameRepositoryJson))
                {
                    Game.GameName = Game.GameName.Split("@")[1];
                }

            }
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var game = _gameRepository.LoadGame(id.Value).Result;
            if (game != null)
            {
                Game = game;
                _gameRepository.DeleteGame(game.GameName);
            }

            return RedirectToPage("./Index");
        }
    }
}
