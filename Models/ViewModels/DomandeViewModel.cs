using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class DomandeViewModel
    {
        
        public int Id{get; set;} 
        public string TestoDomanda {get; set;}
        public int QuizId { get; set; }
       
       public List<RisposteViewModel> Risposte {get;set;}
       public DomandeViewModel()
        {
          
            Risposte = new List<RisposteViewModel>();
            
        }

    
        public static DomandeViewModel FromDataRow(DataRow dataRow)
        {
            var domandeViewModel = new DomandeViewModel {
                Id = Convert.ToInt32(dataRow["Id"]),
                TestoDomanda = Convert.ToString(dataRow["testo_domanda"]),
                QuizId = Convert.ToInt32(dataRow["quiz_id"])
              

            };

            return domandeViewModel;
        }
    }
}