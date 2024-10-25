using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class AggiungiLezioneViewModel
    {
        public CourseViewModel Corso {get;set;}
        public LessonViewModel Lezione {get;set;} 

        public AggiungiLezioneViewModel(){
           Corso = new CourseViewModel();
           Lezione = new LessonViewModel();

        }
    }
}