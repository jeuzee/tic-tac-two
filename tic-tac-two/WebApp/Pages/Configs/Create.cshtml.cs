using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class CreateModel : PageModel
    {
        private readonly IConfigRepository _configRepository;

        public CreateModel(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }
        public string UserName { get; set; } = default!;
        
        [BindProperty]
        public int UserId { get; set; }
        
        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "";
            UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("../Index", new { error = "No username provided." });
            }
    
            return Page();
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost()
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "";
            UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("../Index", new { error = "No username provided." });
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Config.IsGridValid())
            {
                ModelState.AddModelError(string.Empty, "Invalid grid dimensions.");
                return Page(); // Return page with error if validation fails
            }
            
            if (!Config.IsValidWinCondition())
            {
                ModelState.AddModelError(string.Empty, "Invalid win condition.");
                return Page(); // Return page with error if validation fails
            }

            
            
            Config.UserId = UserId;
            Config.CreatedAt = DateTime.Now;
            Config.ModifiedAt = DateTime.Now;

            _configRepository.AddConfig(Config);

            return RedirectToPage("./Index");
        }
    }
}
