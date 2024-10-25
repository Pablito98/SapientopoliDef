using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.Services.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Controllers 
{
    public class MinigiochiController : Controller
    {
        private readonly ILoginService LoginService;

        public MinigiochiController(ILoginService loginService) 
        {
            this.LoginService = loginService;
        }

        public async Task<IActionResult> Memory()
        {
            return View();
        }   

        public async Task<IActionResult> ImpiccatoGame()
        {
            return View();
        } 

        public async Task<IActionResult> Play(int courseId)
        {
            if(courseId == 1)
                return View("ImpiccatoGame");
            else
                return View("Index", "Home");
        } 
    }
}