using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class EditModel : PageModel
    {
        private readonly IConfigRepository _configRepository;

        public EditModel(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        public string UserName { get; set; } = default!;
        
        [BindProperty]
        public string OldName { get; set; } = default!;

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

            var config = _configRepository.GetConfigById(id.Value).Result;
            if (config == null)
            {
                return NotFound();
            }
            Config = config;
            OldName = config.ConfigName;
            
            if (_configRepository.GetType() == typeof(ConfigRepositoryJson))
            {
                Config.ConfigName = Config.ConfigName.Split("@")[1];
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPost()
        {
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


            _configRepository.SaveConfig(ConfigMapper.ConfigToGameConfiguration(Config), OldName);

            return RedirectToPage("./Index");
        }
    }
}
