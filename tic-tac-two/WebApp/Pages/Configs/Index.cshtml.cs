using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using GameBrain;

namespace WebApp.Pages_Configs
{
    public class IndexModel : PageModel
    {
        private readonly IConfigRepository _configRepository;

        public IndexModel(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public IList<Config> Config { get; set; } = default!;

        public string UserName { get; set; } = default!;
        public int UserId { get; set; } = default!;

        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName") ?? "";
            UserId = HttpContext.Session.GetInt32("UserId") ?? -1;
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("../Index", new { error = "No username provided." });
            }

            Config = _configRepository.GetUserConfigsListById(UserId).Result;
            return Page();
        }
    }
}