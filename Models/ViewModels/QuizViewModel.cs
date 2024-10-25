using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class QuizViewModel 
    {

        public int Id{get; set;}
        public string Titolo {get; set;} 
        public string lezioneId {get;set;}
        public List<DomandeViewModel> Domande {get;set;}
     public QuizViewModel()
        {
          
            Domande = new List<DomandeViewModel>();
            
        }
      
        public static QuizViewModel FromDataRow(DataRow dataRow)
        {
            var quizViewModel = new QuizViewModel {
                Id = Convert.ToInt32(dataRow["id"]),
                Titolo = Convert.ToString(dataRow["titolo"]),
                lezioneId = Convert.ToString(dataRow["lezione_id"]),
               

            };

            return quizViewModel;
        }
    }
}