using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.ViewModels;
using Sapientopoli.Models.InputModels;


namespace Sapientopoli.Models.Services.Application
{
    public interface ILoginService
    {
        Task<bool> Registrazione(UtentiViewModel model);
        Task<bool> Accesso(UtentiViewModel model);
        void Logout(UtentiViewModel model);
        Task<List<CourseViewModel>> GetCoursesAsync(CourseListViewModel model);
        Task<List<CourseViewModel>> GetCoursesAdminAsync(SearchListInputModel model);
        Task<CourseDetailProgressViewModel> GetCourseAsync(int id, int userId);
        Task<int> EliminaUtenteAsync(int id);
        Task<UtentiViewModel> RecuperaUtente(int id);
        Task<bool> ModificaUtenteAsync(UtentiViewModel model);
        Task<bool> IscrivitiCorsoAsync(int userId, int courseId);
        Task<bool> ControlloIscrizione(int userId, int courseId);
        Task<List<CourseViewModel>> GetMyCoursesAsync(CourseListViewModel model, int id);
        Task<bool> ControlloRegistrazione(UtentiViewModel model);
        Task<UtentiViewModel> RecuperaDatiView(int id);
        Task<LessonDetailViewModel> GetLessonAsync(int lezioneId, int userId);  
        Task<bool> IsLezioneCompletataAsync(int userId, int lezionePrecedenteId, int corsoId);
        Task SegnalaCompletamentoLezioneAsync(int userId, int corsoId, int totalLezioniCorso);
        Task<int> GetTotalLezioniCorsoAsync(int corsoId);
        Task<int> GetCorsoIdAsync(int lezioneId);
        Task<CourseDetailViewModel> GetCourseAsync(int id);
        Task<bool> DisiscrivitiCorsoAsync(int userId, int courseId); 
        Task<List<UtentiViewModel>> GetUtentiAsync(SearchListInputModel model);
        Task<bool> AddUserAdmin(UtentiViewModel model);
        Task<int> EliminaCorsoAsync(int id);

        Task<LessonDetailViewModel> RecuperaLezioniAdminAsync(int lezioneId);

         Task<bool> AggiungiCorso(CourseViewModel model);

         Task<CourseViewModel> RecuperaCorso(int id);
         Task<bool> ModificaCorsoAsync(CourseViewModel model);

         Task<bool> AggiungiLezione(AggiungiLezioneViewModel model);

         Task<int> EliminaLezione(int id);

         Task<LessonViewModel> RecuperaLezione(int id);

         Task<bool> ModificaLezioneAsync(LessonViewModel model);
    }
}