using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class LessonDetailViewModel : LessonViewModel
    {
        public ProgressiViewModel Progressi {get; set;}
        public List<QuizViewModel> Quiz {get;set;}  
        public LessonViewModel Lezioni {get;set;}
        public List<DomandeViewModel> Domande {get;set;}
        public List<RisposteViewModel> Risposte {get;set;}
        public bool IsLezioneCompletata { get; set; }

 public LessonDetailViewModel()
        {
            Progressi = new ProgressiViewModel();
            Lezioni = new LessonViewModel();
            Quiz = new List<QuizViewModel>();
            Domande = new List<DomandeViewModel>();
            Risposte = new List<RisposteViewModel>();
        }
    }
}