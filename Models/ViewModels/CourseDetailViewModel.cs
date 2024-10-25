using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class CourseDetailViewModel : CourseViewModel
    {

        public string Descrizione { get; set; }
        public List<LessonViewModel> Lezioni { get; set; }
        public TimeSpan TotalCourseDuration
        {
            get => TimeSpan.FromSeconds(Lezioni?.Sum(l => l.Durata.TotalSeconds) ?? 0);
            /*l'operatore null-coalescing ??
            significa che se quello che c'è a sinistra è Null, allora restituisce 0
            altrimenti restituisce quello che c'è a sinistra (cioè la somma)
            */
        }

        public CourseDetailViewModel()
        {
            Lezioni = new List<LessonViewModel>();
        }

         public static new CourseDetailViewModel FromDataRow(DataRow courseRow){ 
            var courseDetailViewModel = new CourseDetailViewModel{
                NomeCorso= Convert.ToString(courseRow["nome_corso"]), //recupero il titolo dal db leggendo la colonna title
                Descrizione = Convert.ToString(courseRow["descrizione"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Rating = Convert.ToDouble(courseRow["rating"]),
                Id = Convert.ToInt32(courseRow["Id"]),
                Lezioni = new List<LessonViewModel>()
            };
            return courseDetailViewModel;   
            } 
    }
}