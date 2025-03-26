using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class DeleteModel : PageModel
    {
        private readonly IConfigRepository _configRepository;


        public DeleteModel(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;
        
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

            var config = _configRepository.GetConfigById(id.Value).Result;

            if (config == null)
            {
                return NotFound();
            }
            else
            {
                Config = config;
                if (_configRepository.GetType() == typeof(ConfigRepositoryJson))
                {
                    Config.ConfigName = Config.ConfigName.Split("@")[1];
                }

            }
            return Page();
        }

        public  IActionResult OnPost(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var config = _configRepository.GetConfigById(id.Value).Result;

            if (config != null)
            {
                Config = config;

                _configRepository.DeleteConfig(Config.ConfigName);
            }

            return RedirectToPage("./Index");
        }
    }
}
