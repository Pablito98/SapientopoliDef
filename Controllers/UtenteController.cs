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

    public class UtenteController : Controller
    {
        private readonly ILoginService LoginService;

        public UtenteController(ILoginService loginService)
        {
            this.LoginService = loginService;
        }

        public IActionResult ListaCorsi()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
        public async Task<IActionResult> IscrizioneCorso(int userId, int courseId)         
        {
            try
            {
                bool iscrizioneResult = await LoginService.IscrivitiCorsoAsync(userId, courseId);
                 string returnUrl = Request.Headers["Referer"].ToString();

            // Se il referer è vuoto o nullo, reindirizza alla Home
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            // Reindirizza l'utente all'URL della pagina precedente
            
                if (iscrizioneResult)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "Iscrizione al corso non riuscita.");
                    return RedirectToAction("Index","Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'inserimento dell'utente.");
            }

            return View("Index","Home");
        }

        public async Task<IActionResult> DisiscrizioneCorso(int userId, int courseId)        
        {
            try
            {
                bool disiscrizioneResult = await LoginService.DisiscrivitiCorsoAsync(userId, courseId);
                 string returnUrl = Request.Headers["Referer"].ToString();

            // Se il referer è vuoto o nullo, reindirizza alla Home
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            // Reindirizza l'utente all'URL della pagina precedente
            
                if (disiscrizioneResult)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "Iscrizione al corso non riuscita.");
                    return RedirectToAction("Index","Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'annullamento dell'iscrizione al corso.");
            }

            return View("Index","Home");
        }

        public async Task<IActionResult> IMieiCorsi(CourseListViewModel model, int id)
        {
            
            List<CourseViewModel> courses = await LoginService.GetMyCoursesAsync(model, id);

            CourseListViewModel viewModel = new CourseListViewModel
            {
                Courses = courses
            };
            
            return View(viewModel);
        }



    }
}