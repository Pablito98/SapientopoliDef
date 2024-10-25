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
using System.IO;
using Microsoft.AspNetCore.Hosting;



namespace Sapientopoli.Controllers
{

    public class AdminController : Controller
    {
         
        private readonly ILoginService LoginService;
      private readonly IWebHostEnvironment WebHostEnvironment;

public AdminController(ILoginService loginService, IWebHostEnvironment webHostEnvironment)
{
    this.LoginService = loginService;
    this.WebHostEnvironment = webHostEnvironment;
}

        public IActionResult Index()
        {
            return View();
        }
      
    
      
        public async Task<IActionResult> GestioneCorsi(SearchListInputModel model)
        {
            string userRole = HttpContext.Session.GetString("ruolo");

            if(userRole=="Admin"){
            List<CourseViewModel> corsi = await LoginService.GetCoursesAdminAsync(model);

            CourseListViewModel viewModel = new CourseListViewModel
            {
                Courses = corsi,
                Ricerca = model
            };

            return View(viewModel);}
            else{
                return RedirectToAction("Index", "Home");
            }
        }

         public async Task<IActionResult> GestioneLezione(int id)
        {
            
             CourseDetailViewModel viewModel = await LoginService.GetCourseAsync(id);
         

             return View("GestioneLezioni", viewModel);
        }
        
         public async Task<IActionResult> GestioneQuiz(int Id)
        {
            
             var model = await LoginService.RecuperaLezioniAdminAsync(Id);
         

             return View(model);
        }


            public async Task<IActionResult> GestioneDomande(int Id)
        {
            
             var model = await LoginService.RecuperaLezioniAdminAsync(Id);
         

             return View(model);
        }


            public async Task<IActionResult> GestioneRisposte(int Id)
        {
            
             var model = await LoginService.RecuperaLezioniAdminAsync(Id);
         

             return View(model);
        }
        public async Task<IActionResult> GestioneUtenti(SearchListInputModel model)
        {
            List<UtentiViewModel> utenti = await LoginService.GetUtentiAsync(model);

            UtentiListViewModel viewModel = new UtentiListViewModel
            {
                Utenti = utenti,
                Ricerca = model
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddUserAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAdmin(UtentiViewModel model)
        {
            try
            {
                bool controllo = await LoginService.ControlloRegistrazione(model);
                if (controllo)
                {
                    Console.WriteLine(model.Nome);
                    bool registrationResult = await LoginService.AddUserAdmin(model);

                    if (registrationResult)
                    {
                        return RedirectToAction("GestioneUtenti", "Admin");
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
            return RedirectToAction("AddUserAdmin", "Admin");

        }


        public IActionResult Error()
        {
            return View("Error!");
        }




        public async Task<IActionResult> EliminaCorso(int id) 

        {
            try
            {
                // Chiamata al metodo del servizio applicativo per eliminare il record
                int rowsAffected = await LoginService.EliminaCorsoAsync(id);

                // Verifica se il record è stato eliminato correttamente
                if (rowsAffected > 0)
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

             [HttpGet]
        public IActionResult AggiungiCorso()
        {
            return View();
        }

      [HttpPost]
public async Task<IActionResult> AggiungiCorso(CourseViewModel model, IFormFile imageFile)
{
    if (ModelState.IsValid)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            // Ottieni il percorso della cartella "img"
            string uploadsFolder = Path.Combine(this.WebHostEnvironment.WebRootPath, "img");

            // Genera un nome di file univoco per evitare sovrascritture
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

            // Combina il percorso completo del file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Salva l'immagine sul server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Aggiorna il percorso dell'immagine nel modello
            model.ImagePath = "/img/" + uniqueFileName;
        }

        // Chiamare il servizio per aggiungere il corso
        bool controllo = await LoginService.AggiungiCorso(model);

        if (controllo)
        {
            return RedirectToAction("GestioneCorsi", "Admin");
        }
    }
    
    // Se il modello non è valido o se il corso non è stato aggiunto correttamente, ritorna alla vista AggiungiCorso
    return View(model);
}

[HttpGet]
         public async Task<IActionResult> ModificaCorso(int id)
        {

            CourseViewModel corso = await LoginService.RecuperaCorso(id);

            // Carica la vista del modulo di iscrizione
            return View(corso);
        }

         [HttpPost]
        public async Task<IActionResult> ModificaCorso(CourseViewModel model, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
        {
            // Ottieni il percorso della cartella "img"
            string uploadsFolder = Path.Combine(this.WebHostEnvironment.WebRootPath, "img");

            // Genera un nome di file univoco per evitare sovrascritture
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

            // Combina il percorso completo del file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Salva l'immagine sul server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Aggiorna il percorso dell'immagine nel modello
            model.ImagePath = "/img/" + uniqueFileName;
        }
        else
        {
            // Se non è stata caricata una nuova immagine, mantieni il percorso dell'immagine esistente nel modello
            string imagePath = model.ImagePath; // Recupera il percorso dell'immagine esistente dal modello
            model.ImagePath = imagePath;
        }
             bool controllo = await LoginService.ModificaCorsoAsync(model);
                if (controllo)
                {
                  return RedirectToAction("GestioneCorsi", "Admin");

                }
           
            return RedirectToAction("ModificaCorso", "Admin");

        }


          [HttpGet]
        public async Task<IActionResult> AggiungiLezione(int id)
        {
            AggiungiLezioneViewModel aggiungiLezione = new AggiungiLezioneViewModel();
           aggiungiLezione.Corso = await LoginService.RecuperaCorso(id);
            
            return View(aggiungiLezione);
        }

      [HttpPost]
public async Task<IActionResult> AggiungiLezione(AggiungiLezioneViewModel model, IFormFile imageFile)
{
    if (ModelState.IsValid)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            // Ottieni il percorso della cartella "img"
            string uploadsFolder = Path.Combine(this.WebHostEnvironment.WebRootPath, "img");

            // Genera un nome di file univoco per evitare sovrascritture
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

            // Combina il percorso completo del file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Salva l'immagine sul server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Aggiorna il percorso dell'immagine nel modello
            model.Lezione.Immagine = "/img/" + uniqueFileName;
        }

        // Chiamare il servizio per aggiungere il corso
        bool controllo = await LoginService.AggiungiLezione(model);

        if (controllo)
        {
            return RedirectToAction("GestioneCorsi", "Admin");
        }
    }
    
    // Se il modello non è valido o se il corso non è stato aggiunto correttamente, ritorna alla vista AggiungiCorso
    return View(model);
}



 public async Task<IActionResult> EliminaLezione(int id) 

        {
            try
            {
                // Chiamata al metodo del servizio applicativo per eliminare il record
                int rowsAffected = await LoginService.EliminaLezione(id);

                // Verifica se il record è stato eliminato correttamente
                if (rowsAffected > 0)
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



[HttpGet]
         public async Task<IActionResult> ModificaLezione(int id)
        {

            LessonViewModel lezione = await LoginService.RecuperaLezione(id);

            // Carica la vista del modulo di iscrizione
            return View(lezione);
        }

         [HttpPost]
        public async Task<IActionResult> ModificaLezione(LessonViewModel model, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
        {
            // Ottieni il percorso della cartella "img"
            string uploadsFolder = Path.Combine(this.WebHostEnvironment.WebRootPath, "img");

            // Genera un nome di file univoco per evitare sovrascritture
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);

            // Combina il percorso completo del file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Salva l'immagine sul server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Aggiorna il percorso dell'immagine nel modello
            model.Immagine = "/img/" + uniqueFileName;
        }
        else
        {
            // Se non è stata caricata una nuova immagine, mantieni il percorso dell'immagine esistente nel modello
            string imagePath = model.Immagine; // Recupera il percorso dell'immagine esistente dal modello
            model.Immagine = imagePath;
        }
             bool controllo = await LoginService.ModificaLezioneAsync(model);
                if (controllo)
                {
                  return RedirectToAction("Gestione", "Admin");

                }
           
            return RedirectToAction("ModificaLezione", "Admin");

        }
    }
}