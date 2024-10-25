using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.Services.Application;
using Sapientopoli.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService LoginService;

        public HomeController(ILoginService loginService) 
        {
            this.LoginService = loginService;
        }

        
        public async Task<IActionResult> Index(int id)
        {
            UtentiViewModel utente = await LoginService.RecuperaDatiView(id);
            return View(utente);
        }

        public async Task<IActionResult> IndexUtente(UtentiViewModel model)
        {
            UtentiViewModel utente = await LoginService.RecuperaDatiView(model.Id);
            return View(utente);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
