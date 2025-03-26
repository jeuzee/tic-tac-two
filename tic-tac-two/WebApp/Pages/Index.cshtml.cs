using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public string? UserName { get; set; }

    [BindProperty (SupportsGet = true)]
    public string? Error { get; set; }
    

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        UserName = UserName?.Trim();
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            HttpContext.Session.SetString("UserName", UserName);
            return RedirectToPage("./Home", new { userName = UserName });
        }

        Error = "Please enter a username.";
        return Page();
    }
}