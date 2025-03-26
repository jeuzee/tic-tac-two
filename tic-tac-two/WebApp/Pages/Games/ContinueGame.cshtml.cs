using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp;

public class ContinueGame : PageModel
{
    public string UserName { get; set; } = default!;
    
    public SelectList GameModes { get; set; } = default!;
    
    [BindProperty]
    public EGameMode GameMode { get; set; }

    [BindProperty(SupportsGet = true)]
    public int GameId { get; set; }
    
    public IActionResult OnGet()
    {
        UserName = HttpContext.Session.GetString("UserName") ?? "";
        if (string.IsNullOrEmpty(UserName))
        {
            return RedirectToPage("./Index", new { error = "No username provided." });
        }

        GameModes = new SelectList(Enum.GetValues(typeof(EGameMode)).Cast<EGameMode>());
        
        return Page();
    }

    public IActionResult OnPost()
    {
        var selectedGameMode = Enum.Parse<EGameMode>(Request.Form["GameMode"]!);

        return RedirectToPage("../PlayGame", new { gameId = GameId, gameMode = selectedGameMode });
    }
}