using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.Services.Infrastructure;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.Services.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Models.Services.Application
{
    public class AdoNetLoginService : ILoginService
    {
        private readonly IDatabaseAccessor db;
        private readonly IHttpContextAccessor httpContextAccessor; //sessione...?

        public AdoNetLoginService(IDatabaseAccessor db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Registrazione(UtentiViewModel model)
        { //aggiugnere controllo mail diverse

            string dataDiNascitaFormatted = model.DataDiNascita.ToString("dd/MM/yyyy");
            FormattableString query = $"INSERT INTO utenti (nome, email, password, cognome, genere, numero_telefono, data_di_nascita) VALUES ({model.Nome}, {model.Email}, {model.Password},{model.Cognome},{model.Genere},{model.NumeroDiTelefono},{dataDiNascitaFormatted})";

            try
            {
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddUserAdmin(UtentiViewModel model)
        { //aggiugnere controllo mail diverse

            string dataDiNascitaFormatted = model.DataDiNascita.ToString("dd/MM/yyyy");
            FormattableString query = $"INSERT INTO utenti (nome, email, password, cognome, genere, numero_telefono, data_di_nascita,Ruolo) VALUES ({model.Nome}, {model.Email}, {model.Password},{model.Cognome},{model.Genere},{model.NumeroDiTelefono},{dataDiNascitaFormatted},{model.Ruolo})";

            try
            {
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Accesso(UtentiViewModel model)
        {

            FormattableString query = $@"SELECT COUNT(*) FROM utenti WHERE email = {model.Email} AND password = {model.Password}; SELECT id, Ruolo FROM utenti WHERE email = {model.Email} AND password = {model.Password}" ;

            try
            {
                DataSet dataSet = await db.QueryAsync(query);

                int count = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
                //int userId = Convert.ToInt32(dataSet.Tables[0].Rows[0][1]);

                // Salva l'ID utente nella sessione

                // Verifica se il risultato contiene almeno una riga
                // Se sì, significa che le credenziali sono corrette e l'utente può accedere

                if (count > 0)
                {
                    int userId = Convert.ToInt32(dataSet.Tables[1].Rows[0]["id"]);
                    string userRole = dataSet.Tables[1].Rows[0]["ruolo"].ToString();
                    httpContextAccessor.HttpContext.Session.SetInt32("id", userId);
                    httpContextAccessor.HttpContext.Session.SetString("ruolo", userRole); // Salva il ruolo nella sessione
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async void Logout(UtentiViewModel model)
        {
            httpContextAccessor.HttpContext.Session.Clear();
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(CourseListViewModel model)
        {
            FormattableString query = @$"SELECT * FROM corsi;";
            //con LIKE e {"%"+search+"%"}"  gli diciamo che nella condizione search deve essere contenuto nei titoli dei corsi

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0]; //recupera la prima tabella del dataset
            var courseList = new List<CourseViewModel>(); //crea la lista di corsi che deve eseere passata all view
            // per ogni riga presente nalla datatable deve creare un oggetto di tipo CourseViewModel 
            foreach (DataRow courseRow in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(course);
            }

            return courseList;
        }

                public async Task<List<CourseViewModel>> GetCoursesAdminAsync(SearchListInputModel model)
        {
            FormattableString query = @$"SELECT * FROM corsi WHERE nome_corso LIKE {"%" + model.Search + "%"};";
            //con LIKE e {"%"+search+"%"}"  gli diciamo che nella condizione search deve essere contenuto nei titoli dei corsi

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0]; //recupera la prima tabella del dataset
            var courseList = new List<CourseViewModel>(); //crea la lista di corsi che deve eseere passata all view
            // per ogni riga presente nalla datatable deve creare un oggetto di tipo CourseViewModel 
            foreach (DataRow courseRow in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(course);
            }

            return courseList;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            FormattableString query = $@"SELECT * FROM corsi WHERE Id={id};
                                        SELECT * FROM lezioni WHERE corso_id={id};";
            
            DataSet dataSet = await db.QueryAsync(query);
            var courseTable = dataSet.Tables[0];
            var lessonTable = dataSet.Tables[1];


            if (courseTable.Rows.Count != 1)
            {
                throw new InvalidOperationException($"Did not return exactly 1 row for Course {id}");
            }

            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            

            for (int i = 0; i < lessonTable.Rows.Count; i++)
            {
                DataRow lessonRow = lessonTable.Rows[i];
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                

                courseDetailViewModel.Lezioni.Add(lessonViewModel);
            };

            return courseDetailViewModel;
        }


        public async Task<CourseDetailProgressViewModel> GetCourseAsync(int id, int userId)
        {
            FormattableString query = $@"SELECT * FROM corsi WHERE Id={id};
                                        SELECT * FROM lezioni WHERE corso_id={id};
                                        SELECT lezioni_completate FROM progressi WHERE id_utente={userId} AND id_corso={id};
                                        SELECT * FROM progressi WHERE id_utente={userId} AND id_corso={id};";

            DataSet dataSet = await db.QueryAsync(query);
            var courseTable = dataSet.Tables[0];
            var lessonTable = dataSet.Tables[1];
            var progressTable = dataSet.Tables[2];
            var completeProgressTable = dataSet.Tables[3];

            if (courseTable.Rows.Count != 1)
            {
                throw new InvalidOperationException($"Did not return exactly 1 row for Course {id}");
            }

            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            var completedLessonIds = new List<int>();
            if (progressTable.Rows.Count > 0)
            {
                var completedLessonsString = progressTable.Rows[0]["lezioni_completate"].ToString();
                completedLessonIds = completedLessonsString.Split(',').Select(int.Parse).ToList();
            }

            for (int i = 0; i < lessonTable.Rows.Count; i++)
            {
                DataRow lessonRow = lessonTable.Rows[i];
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                lessonViewModel.IsPrimaLezione = (i == 0);
                lessonViewModel.IsLezionePrecedenteCompletata = (i == 0 || completedLessonIds.Contains(Convert.ToInt32(lessonTable.Rows[i - 1]["id"])));

                courseDetailViewModel.Lezioni.Add(lessonViewModel);
            }

            ProgressiViewModel progressiViewModel = null;
            if (completeProgressTable.Rows.Count > 0)
            {
                var progressRow = completeProgressTable.Rows[0];
                progressiViewModel = ProgressiViewModel.FromDataRow(progressRow);
            }
            else
            {
                // Gestisci il caso in cui non ci sono dati nella tabella `progressi`
                progressiViewModel = new ProgressiViewModel
                {
                    IdUtente = userId,
                    IdCorso = id,
                    LezioniCompletate = 0
                };
            }

            var courseDetailProgressViewModel = new CourseDetailProgressViewModel()
            {
                Dettagli = courseDetailViewModel,
                Progressi = progressiViewModel
            };

            return courseDetailProgressViewModel;
        }




        public async Task<int> EliminaUtenteAsync(int id)
        {
            try
            {
                FormattableString query = @$"DELETE FROM progressi WHERE id_utente={id};
                                            DELETE FROM utenti WHERE id={id};";

                // Eseguire la query
                string role = httpContextAccessor.HttpContext.Session.GetString("ruolo");
                await db.QueryAsync(query);
                
                if (role == "User")
                {
                    httpContextAccessor.HttpContext.Session.Clear();
                }

                return 1; // Ritorna un valore int per indicare l'avvenuta eliminazione
            }
            catch
            {
                return 0; // Ritorna 0 se si verifica un errore durante l'eliminazione
            }

        }

        public async Task<int> EliminaCorsoAsync(int id)
        {
            try
            {
                FormattableString query = @$"DELETE FROM progressi WHERE id_corso={id};
                                             DELETE FROM risposte WHERE domanda_id IN (SELECT id FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id IN (SELECT id FROM lezioni WHERE corso_id={id})));
                                             DELETE FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id IN (SELECT id FROM lezioni WHERE corso_id={id}));
                                             DELETE FROM quiz WHERE lezione_id IN (SELECT id FROM lezioni WHERE corso_id={id});
                                             DELETE FROM lezioni WHERE corso_id={id};
                                             DELETE FROM corsi WHERE id={id};";
                // Eseguire la query
                await db.QueryAsync(query);

                return 1; // Ritorna un valore int per indicare l'avvenuta eliminazione
            }
            catch
            {
                return 0; // Ritorna 0 se si verifica un errore durante l'eliminazione
            }

        }


        public async Task<UtentiViewModel> RecuperaUtente(int id)
        {
            FormattableString query = $"SELECT * FROM utenti WHERE id={id}";

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            UtentiViewModel utente = new UtentiViewModel();

            foreach (DataRow utenteRow in dataTable.Rows)
            {
                utente = UtentiViewModel.FromDataRow(utenteRow);

            }

            return utente;
        }


        public async Task<bool> ModificaUtenteAsync(UtentiViewModel model)
        {
            FormattableString query = $@"UPDATE utenti SET 
            nome = {model.Nome}, cognome={model.Cognome}, email= {model.Email}, password = {model.Password}, numero_telefono = {model.NumeroDiTelefono}, genere = {model.Genere}, data_di_nascita={model.DataDiNascita}, Ruolo={model.Ruolo}
            WHERE id = {model.Id}";

            try
            {
                await db.QueryAsync(query);
                return true; // Ritorna true se la registrazione ha avuto successo
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni, ad esempio registrando un errore o lanciando un'eccezione personalizzata
                Console.WriteLine($"Errore durante l'aggiornamento dell'utente: {ex.Message}");
                return false; // Ritorna false se la registrazione ha fallito
            }
        }

        public async Task<bool> IscrivitiCorsoAsync(int userId, int courseId)
        {
            try
            {
                // Controlla se l'utente è già iscritto al corso
                bool isIscritto = await ControlloIscrizione(userId, courseId);
                if (isIscritto)
                {
                    Console.WriteLine("L'utente è già iscritto a questo corso.");
                    return false;
                }

                // Se l'utente non è già iscritto, procedi con l'iscrizione
                FormattableString query = $@"INSERT INTO progressi (id_utente, id_corso, progresso)
                                            VALUES ({userId}, {courseId}, 0.0)";
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DisiscrivitiCorsoAsync(int userId, int courseId)
        {
            try
            {
                // Se l'utente non è già iscritto, procedi con l'iscrizione
                FormattableString query = $@"DELETE FROM progressi WHERE id_utente={userId} AND id_corso={courseId};";
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'eliminazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ControlloIscrizione(int userId, int courseId)
        {
            try
            {
                FormattableString query = $"SELECT COUNT(*) FROM progressi WHERE id_utente = {userId} AND id_corso = {courseId}";

                DataSet dataSet = await db.QueryAsync(query);
                int count = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il controllo dell'iscrizione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<List<CourseViewModel>> GetMyCoursesAsync(CourseListViewModel model, int id)
        {
            FormattableString query = $@"SELECT corsi.*, progressi.progresso
                FROM corsi 
                INNER JOIN progressi ON corsi.id = progressi.id_corso
                INNER JOIN utenti ON utenti.id = progressi.id_utente
                WHERE utenti.id ={id}";

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            var courseList = new List<CourseViewModel>();
            foreach (DataRow courseRow in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(course);
            }

            return courseList;
        }

        public async Task<bool> ControlloRegistrazione(UtentiViewModel model)
        {
            FormattableString query = $@"SELECT COUNT(*) FROM utenti WHERE email = {model.Email}";

            try
            {
                DataSet dataSet = await db.QueryAsync(query);

                int count = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);

                if (count > 0)
                {

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione dell'utente: {ex.Message}");
                return false;
            }
        }

        public async Task<UtentiViewModel> RecuperaDatiView(int id)
        {
            FormattableString query = $@"SELECT * FROM utenti WHERE id={id}";

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            UtentiViewModel utente = new UtentiViewModel();

            foreach (DataRow utenteRow in dataTable.Rows)
            {
                utente = UtentiViewModel.FromDataRow(utenteRow);

            }

            return utente;
        }

        public async Task<LessonDetailViewModel> GetLessonAsync(int lezioneId, int userId)
        {
            FormattableString query = $@"
                SELECT * FROM lezioni WHERE id={lezioneId};
                SELECT * FROM quiz WHERE lezione_id={lezioneId};
                SELECT * FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id={lezioneId});
                SELECT * FROM risposte WHERE domanda_id IN (SELECT id FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id={lezioneId}));
                SELECT * FROM progressi WHERE id_corso IN (SELECT corso_id FROM lezioni WHERE id={lezioneId}) AND id_utente={userId};
            ";

            DataSet dataSet = await db.QueryAsync(query); 

            var lezioniTable = dataSet.Tables[0];
            var quizTable = dataSet.Tables[1];
            var domandeTable = dataSet.Tables[2];
            var risposteTable = dataSet.Tables[3];
            var progressiTable = dataSet.Tables[4];

            var lessonDetailViewModel = new LessonDetailViewModel();

            var lezioniRow = lezioniTable.Rows[0];
            var lessonViewModel = LessonViewModel.FromDataRow(lezioniRow);
            lessonDetailViewModel.Lezioni = lessonViewModel;

            var quizDict = new Dictionary<int, QuizViewModel>();
            foreach (DataRow quizRow in quizTable.Rows)
            {
                var quizViewModel = QuizViewModel.FromDataRow(quizRow);
                quizViewModel.Domande = new List<DomandeViewModel>();  // Inizializza la lista delle domande
                quizDict.Add(quizViewModel.Id, quizViewModel);
            }

            var domandeDict = new Dictionary<int, DomandeViewModel>();
            foreach (DataRow domandeRow in domandeTable.Rows)
            {
                var domandeViewModel = DomandeViewModel.FromDataRow(domandeRow);
                domandeViewModel.Risposte = new List<RisposteViewModel>();  // Inizializza la lista delle risposte
                domandeDict.Add(domandeViewModel.Id, domandeViewModel);

                if (quizDict.TryGetValue(domandeViewModel.QuizId, out var quizViewModel))
                {
                    quizViewModel.Domande.Add(domandeViewModel);
                }
            }

            foreach (DataRow risposteRow in risposteTable.Rows)
            {
                var risposteViewModel = RisposteViewModel.FromDataRow(risposteRow);
                if (domandeDict.TryGetValue(risposteViewModel.DomandaId, out var domandeViewModel))
                {
                    domandeViewModel.Risposte.Add(risposteViewModel);
                }
            }

            lessonDetailViewModel.Quiz = quizDict.Values.ToList();


            ProgressiViewModel progressiViewModel = null;
            if (progressiTable.Rows.Count > 0)
            {
                var progressRow = progressiTable.Rows[0];
                progressiViewModel = ProgressiViewModel.FromDataRow(progressRow);
            }
            else
            {
                // Gestisci il caso in cui non ci sono dati nella tabella `progressi`
                progressiViewModel = new ProgressiViewModel
                {
                    IdUtente = 0,
                    IdCorso = 0,
                    LezioniCompletate = 0
                };
            }

            lessonDetailViewModel.Progressi = progressiViewModel;

            return lessonDetailViewModel;
        }

        public async Task<bool> IsLezioneCompletataAsync(int userId, int lezionePrecedenteId, int corsoId)
        {
            FormattableString query = $@"
                SELECT lezioni_completate FROM progressi 
                WHERE id_utente = {userId} AND id_corso = {corsoId};
                SELECT sequenza FROM lezioni WHERE id = {lezionePrecedenteId}";
            DataSet dataSet = await db.QueryAsync(query);

            var dataTable = dataSet.Tables[0];
            var sequenzaTable = dataSet.Tables[1];

            // Verifica se esiste un risultato
            if (dataTable.Rows.Count > 0)
            {
                // Ottieni il valore dell'ID del corso dalla prima riga del risultato
                int lezioniCompletate = Convert.ToInt32(dataTable.Rows[0]["lezioni_completate"]);
                int sequenza = Convert.ToInt32(sequenzaTable.Rows[0]["sequenza"]);
                if(lezioniCompletate > sequenza)
                    return true;
                else
                    return false;
            }

            else
            {
                // Se non ci sono risultati, restituisci un valore predefinito o gestisci l'eccezione di conseguenza
                throw new InvalidOperationException($"Nessun corso trovato per la lezione con ID {lezionePrecedenteId}");
            }
        }

        public async Task<int> GetTotalLezioniCorsoAsync(int corsoId)
        {
            FormattableString query = $"SELECT COUNT(*) FROM lezioni WHERE corso_id = {corsoId}";
            DataSet dataSet = await db.QueryAsync(query);
            int count = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
            return count;
        }

        public async Task<int> GetCorsoIdAsync(int lezioneId)
        {
            FormattableString query = $"SELECT corso_id FROM lezioni WHERE id = {lezioneId}";
            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];


            // Verifica se esiste un risultato
            if (dataTable.Rows.Count > 0)
            {
                // Ottieni il valore dell'ID del corso dalla prima riga del risultato
                int corsoId = Convert.ToInt32(dataTable.Rows[0]["corso_id"]);
                return corsoId;
            }

            else
            {
                // Se non ci sono risultati, restituisci un valore predefinito o gestisci l'eccezione di conseguenza
                throw new InvalidOperationException($"Nessun corso trovato per la lezione con ID {lezioneId}");
            }
        }

        public async Task SegnalaCompletamentoLezioneAsync(int userId, int corsoId, int totalLezioniCorso)
        {
            // Calcola la percentuale di completamento del corso
            decimal percentualeCompletamento = (decimal)100 / totalLezioniCorso;

            // Aggiorna lo stato di completamento della lezione per l'utente nel database
            await db.QueryAsync($"UPDATE progressi SET progresso = progresso + {percentualeCompletamento}, lezioni_completate = lezioni_completate + 1 WHERE id_utente = {userId} AND id_corso={corsoId} ");
        }

        public async Task<List<UtentiViewModel>> GetUtentiAsync(SearchListInputModel model)
        {
            FormattableString query = $"SELECT *  FROM utenti WHERE nome LIKE {"%" + model.Search + "%"}";
            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0]; //recupera la prima tabella del dataset
            var utentiList = new List<UtentiViewModel>(); //crea la lista di utenti che deve essere passata all view
            // per ogni riga presente nalla datatable deve creare un oggetto di tipo UtentiViewModel 
            foreach (DataRow utenteRow in dataTable.Rows)
            {
                UtentiViewModel utente = UtentiViewModel.FromDataRow(utenteRow);
                utentiList.Add(utente);
            }
            return utentiList;
        }


         public async Task<LessonDetailViewModel> RecuperaLezioniAdminAsync(int lezioneId)
        {
            FormattableString query = $@"
                SELECT * FROM lezioni WHERE id={lezioneId};
                SELECT * FROM quiz WHERE lezione_id={lezioneId};
                SELECT * FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id={lezioneId});
                SELECT * FROM risposte WHERE domanda_id IN (SELECT id FROM domande WHERE quiz_id IN (SELECT id FROM quiz WHERE lezione_id={lezioneId}));
            ";

            DataSet dataSet = await db.QueryAsync(query); 

            var lezioniTable = dataSet.Tables[0];
            var quizTable = dataSet.Tables[1];
            var domandeTable = dataSet.Tables[2];
            var risposteTable = dataSet.Tables[3];
            

            var lessonDetailViewModel = new LessonDetailViewModel();

            var lezioniRow = lezioniTable.Rows[0];
            var lessonViewModel = LessonViewModel.FromDataRow(lezioniRow);
            lessonDetailViewModel.Lezioni = lessonViewModel;

            var quizDict = new Dictionary<int, QuizViewModel>();
            foreach (DataRow quizRow in quizTable.Rows)
            {
                var quizViewModel = QuizViewModel.FromDataRow(quizRow);
                quizViewModel.Domande = new List<DomandeViewModel>();  // Inizializza la lista delle domande
                quizDict.Add(quizViewModel.Id, quizViewModel);
            }

            var domandeDict = new Dictionary<int, DomandeViewModel>();
            foreach (DataRow domandeRow in domandeTable.Rows)
            {
                var domandeViewModel = DomandeViewModel.FromDataRow(domandeRow);
                domandeViewModel.Risposte = new List<RisposteViewModel>();  // Inizializza la lista delle risposte
                domandeDict.Add(domandeViewModel.Id, domandeViewModel);

                if (quizDict.TryGetValue(domandeViewModel.QuizId, out var quizViewModel))
                {
                    quizViewModel.Domande.Add(domandeViewModel);
                }
            }

            foreach (DataRow risposteRow in risposteTable.Rows)
            {
                var risposteViewModel = RisposteViewModel.FromDataRow(risposteRow);
                if (domandeDict.TryGetValue(risposteViewModel.DomandaId, out var domandeViewModel))
                {
                    domandeViewModel.Risposte.Add(risposteViewModel);
                }
            }

            lessonDetailViewModel.Quiz = quizDict.Values.ToList();
 

            return lessonDetailViewModel;
        }


        public async Task<bool> AggiungiCorso(CourseViewModel model)
        {
                 FormattableString query = $"INSERT INTO corsi (nome_corso, descrizione, ImagePath, rating) VALUES ({model.NomeCorso}, {model.Descrizione}, {model.ImagePath},{model.Rating})";

            try
            {
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione del corso: {ex.Message}");
                return false;
            }
        }


         public async Task<CourseViewModel> RecuperaCorso(int id)
        {
            FormattableString query = $@"SELECT * FROM corsi WHERE Id={id};";
            
            DataSet dataSet = await db.QueryAsync(query);
            var courseTable = dataSet.Tables[0];


           

            var courseRow = courseTable.Rows[0];
            var courseViewModel = CourseViewModel.FromDataRow(courseRow);

            

          

            return courseViewModel;
        }

         public async Task<bool> ModificaCorsoAsync(CourseViewModel model)
        { if(model.ImagePath==null){
            FormattableString query = $@"UPDATE corsi SET 
            nome_corso = {model.NomeCorso}, descrizione = {model.Descrizione}, rating={model.Rating}
            WHERE id = {model.Id}";
            
             try
            {
                await db.QueryAsync(query);
                return true; // Ritorna true se la registrazione ha avuto successo
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni, ad esempio registrando un errore o lanciando un'eccezione personalizzata
                Console.WriteLine($"Errore durante l'aggiornamento dell'utente: {ex.Message}");
                return false; // Ritorna false se la registrazione ha fallito
            }
            
            }
            else{
            FormattableString query = $@"UPDATE corsi SET 
            nome_corso = {model.NomeCorso}, descrizione = {model.Descrizione}, ImagePath ={model.ImagePath}, rating={model.Rating}
            WHERE id = {model.Id}";
            try
            {
                await db.QueryAsync(query);
                return true; // Ritorna true se la registrazione ha avuto successo
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni, ad esempio registrando un errore o lanciando un'eccezione personalizzata
                Console.WriteLine($"Errore durante l'aggiornamento dell'utente: {ex.Message}");
                return false; // Ritorna false se la registrazione ha fallito
            }
            }
            
        }


         public async Task<bool> AggiungiLezione(AggiungiLezioneViewModel model)
        {
                 FormattableString query = $"INSERT INTO lezioni (corso_id,titolo, descrizione,teoria,video,sequenza,immagine) VALUES ({model.Lezione.CorsoId},{model.Lezione.Titolo}, {model.Lezione.Descrizione},{model.Lezione.Teoria},{model.Lezione.Video},{model.Lezione.Sequenza}, {model.Lezione.Immagine})";

            try
            {
                await db.QueryAsync(query);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la registrazione del corso: {ex.Message}");
                return false;
            }
        }


        public async Task<int> EliminaLezione(int id)
        {
            
             try
            {
                FormattableString query = @$"DELETE FROM lezioni WHERE id={id}";
                // Eseguire la query
                await db.QueryAsync(query);

                return 1; // Ritorna un valore int per indicare l'avvenuta eliminazione
            }
            catch
            {
                return 0; // Ritorna 0 se si verifica un errore durante l'eliminazione
            }
        }
  

       public async Task<LessonViewModel> RecuperaLezione(int id)
       {
             FormattableString query = $"SELECT * FROM lezioni WHERE id={id}";

            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            LessonViewModel lezione = new LessonViewModel();

            foreach (DataRow lezioneRow in dataTable.Rows)
            {
                lezione = LessonViewModel.FromDataRow(lezioneRow);

            }

            return lezione;
       }
  

 public async Task<bool> ModificaLezioneAsync(LessonViewModel model)
        { if(model.Immagine==null){
            FormattableString query = $@"UPDATE lezioni SET 
            titolo = {model.Titolo}, descrizione = {model.Descrizione}, teoria={model.Teoria}, video={model.Video}, durata={model.Durata}
            WHERE id = {model.Id}";
            
             try
            {
                await db.QueryAsync(query);
                return true; // Ritorna true se la registrazione ha avuto successo
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni, ad esempio registrando un errore o lanciando un'eccezione personalizzata
                Console.WriteLine($"Errore durante l'aggiornamento dell'utente: {ex.Message}");
                return false; // Ritorna false se la registrazione ha fallito
            }
            
            }
            else{
             FormattableString query = $@"UPDATE lezioni SET 
            titolo = {model.Titolo}, descrizione = {model.Descrizione}, teoria={model.Teoria}, video={model.Video}, durata={model.Durata}, immagine ={model.Immagine}
            WHERE id = {model.Id}";
            try
            {
                await db.QueryAsync(query);
                return true; // Ritorna true se la registrazione ha avuto successo
            }
            catch (Exception ex)
            {
                // Gestisci eventuali eccezioni, ad esempio registrando un errore o lanciando un'eccezione personalizzata
                Console.WriteLine($"Errore durante l'aggiornamento dell'utente: {ex.Message}");
                return false; // Ritorna false se la registrazione ha fallito
            }
            }
            
        }
  
  
    }
}