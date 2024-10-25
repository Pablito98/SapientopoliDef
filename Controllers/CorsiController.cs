using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.Services.Application;
using Sapientopoli.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Controllers
{
    public class CorsiController : Controller
    {
         private readonly ILoginService LoginService;
        public CorsiController(ILoginService loginService)
        {
            this.LoginService = loginService;
        }
        public async Task<IActionResult> ListaCorsi(CourseListViewModel model)
        {
            
            List<CourseViewModel> courses = await LoginService.GetCoursesAsync(model);

             CourseListViewModel viewModel = new CourseListViewModel
            {
                Courses = courses
            };
            ViewData["Title"] = "Elenco dei corsi";
            return View(viewModel);
        }

        /* public async Task<IActionResult> Detail(int id)
        {
           CourseDetailViewModel viewModel =await LoginService.GetCourseAsync(id);            
            
            return View(viewModel);          
        }*/

        public async Task<IActionResult> Detail(int id, int userId)
        {
            // Ottieni i dettagli del corso
           if(userId == 0){
                CourseDetailViewModel viewModel = await LoginService.GetCourseAsync(id);
                return View("DetailOspite", viewModel);
           }
           else{
            CourseDetailProgressViewModel viewModel = await LoginService.GetCourseAsync(id, userId);
            bool isIscritto = await LoginService.ControlloIscrizione(userId, id);

            // Passa lo stato di iscrizione alla vista
            ViewBag.IsIscritto = isIscritto;

            return View(viewModel); 
           }

           
        /*
        public async Task<IActionResult> Detail(int id)
        {
            int userId = HttpContext.Session.GetInt32("id") ?? 0; // Ottieni l'ID dell'utente dalla sessione
            CourseDetailViewModel viewModel = await LoginService.GetCourseAsync(id, userId);

            bool isIscritto = await LoginService.ControlloIscrizione(userId, id);
            ViewBag.IsIscritto = isIscritto;

            return View(viewModel);
        }*/
        }


        [HttpPost]
        public async Task<IActionResult> SegnalaCompletamentoLezione(int lezioneId)
        {
            // Segnala il completamento della lezione per l'utente loggato
            int userId = HttpContext.Session.GetInt32("id") ?? 0; // Ottieni l'ID dell'utente dalla sessione
            int corsoId = await LoginService.GetCorsoIdAsync(lezioneId);
            int totalLezioniCorso = await LoginService.GetTotalLezioniCorsoAsync(corsoId);
            await LoginService.SegnalaCompletamentoLezioneAsync(userId, corsoId, totalLezioniCorso);
            
            // Reindirizza l'utente alla pagina della lezione o ad un'altra destinazione
            return RedirectToAction("Lezione", new { lezioneId = lezioneId });
        }


        
    public async Task<IActionResult> Lezione(int lezioneId)
    {
        // Ottieni l'ID dell'utente dalla sessione
        int userId = HttpContext.Session.GetInt32("id") ?? 0;

        // Ottieni la lezione corrente
        var lessonDetail = await LoginService.GetLessonAsync(lezioneId, userId);


        // Verifica se esiste una lezione precedente
        int lezionePrecedenteId = lessonDetail.Lezioni.Sequenza - 1;
        //if (lezionePrecedenteId > 0)
        //{
            int corsoId = await LoginService.GetCorsoIdAsync(lezioneId);
            // Verifica se l'utente ha completato la lezione precedente
            bool lezionePrecedenteCompletata;
            if (lezionePrecedenteId > 0)
            {
                lezionePrecedenteCompletata = await LoginService.IsLezioneCompletataAsync(userId, lezionePrecedenteId, corsoId);
            }                
            
        //} 
            return View(lessonDetail);
        }
    } 
}