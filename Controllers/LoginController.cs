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
    public class LoginController : Controller
    {
        private readonly ILoginService LoginService;

        public LoginController(ILoginService loginService)
        {
            this.LoginService = loginService;
        }

        [HttpGet]
        public IActionResult Registrazione()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrazione(UtentiViewModel model)
        {
            try
            {
                bool controllo = await LoginService.ControlloRegistrazione(model);
                if (controllo)
                {
                    Console.WriteLine(model.Nome);
                    bool registrationResult = await LoginService.Registrazione(model);

                    if (registrationResult)
                    {
                        return RedirectToAction("Accesso", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Inserimento utente non riuscito.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'inserimento dell'utente.");
            }
            return RedirectToAction("Registrazione", "Login");

        }



        [HttpGet]
        public IActionResult Accesso()
        {
            
            return View("Accesso");
        }
        
        [HttpPost]
        public async Task<IActionResult> Accesso(UtentiViewModel model)
        {
            try
            {
                bool registrationResult = await LoginService.Accesso(model);
                
                if (registrationResult)
                {   
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Gestisci il fallimento dell'inserimento
                    return RedirectToAction("Registrazione", "Login");
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni durante l'inserimento nell'utente nel database
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'inserimento dell'utente.");
            }

            return RedirectToAction("Index", "Home");

        }

        /*
        [HttpPost]
        public IActionResult Logout(UtentiViewModel model)
        {
            LoginService.Logout(model);
            return RedirectToAction("Index", "Home");
        }
        */

        [HttpPost]
        public IActionResult Logout(UtentiViewModel model)
        {
            string userRole = HttpContext.Session.GetString("ruolo");
            // Esegui il logout
            LoginService.Logout(model);

            // Ottieni l'URL della pagina precedente dal referer HTTP
            string returnUrl = Request.Headers["Referer"].ToString();

            // Se il referer è vuoto o nullo, reindirizza alla Home
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            if(userRole == "User")
                // Reindirizza l'utente all'URL della pagina precedente
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
 
        public async Task<IActionResult> EliminaUtente(int id) 

        {
            string userRole = HttpContext.Session.GetString("ruolo");
            try
            {
                // Chiamata al metodo del servizio applicativo per eliminare il record
                int rowsAffected = await LoginService.EliminaUtenteAsync(id);

                // Verifica se il record è stato eliminato correttamente
                if (rowsAffected > 0)
                {
                    // Il record è stato eliminato con successo, esegui un'azione appropriata (reindirizzamento, messaggio, ecc.)
                    if(userRole == "Admin")
                    {
                        // Ottieni l'URL della pagina precedente dal referer HTTP
                        string returnUrl = Request.Headers["Referer"].ToString();

                        // Se il referer è vuoto o nullo, reindirizza alla Home
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            returnUrl = Url.Action("Index", "Home");
                        }

                        // Reindirizza l'utente all'URL della pagina precedente
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    return RedirectToAction("Accesso");
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni durante l'eliminazione del record
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'eliminazione del record.");
                return View("Accesso"); // Esempio: reindirizza alla pagina di errore
            }
        }

        public async Task<IActionResult> Update(int id)
        {

            UtentiViewModel utente = await LoginService.RecuperaUtente(id);

            // Carica la vista del modulo di iscrizione
            return View(utente);
        }

        [HttpPost]
        public async Task<IActionResult> ModificaUtente(UtentiViewModel model)
        {
            string userRole = HttpContext.Session.GetString("ruolo");
            try
            {
                // Chiamata al metodo del servizio applicativo per eliminare il record
                bool rowsAffected = await LoginService.ModificaUtenteAsync(model);

                // Verifica se il record è stato eliminato correttamente
                if (rowsAffected)
                {
                    if(userRole == "Admin")
                    {
                        return RedirectToAction("GestioneUtenti","Admin");
                    }
                    // Il record è stato modificato con successo, esegui un'azione appropriata (reindirizzamento, messaggio, ecc.)
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    return RedirectToAction("Accesso");
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni durante l'eliminazione del record
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'aggiornamento del record.");
                return View("Accesso"); // Esempio: reindirizza alla pagina di errore
            }


        }

    }
}
