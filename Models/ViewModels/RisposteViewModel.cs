using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class RisposteViewModel
    {
        
        public int Id{get; set;}
        public string TestoRisposta {get; set;} 
        public int DomandaId { get; set; }
       public bool IsCorretta {get;set;}
          

        public static RisposteViewModel FromDataRow(DataRow dataRow)
        {
            var risposteViewModel = new RisposteViewModel {
                Id = Convert.ToInt32(dataRow["Id"]),
                TestoRisposta = Convert.ToString(dataRow["testo_risposta"]),
                 IsCorretta = Convert.ToBoolean(dataRow["is_corretta"]),
                
                DomandaId = Convert.ToInt32(dataRow["domanda_id"])

            };

            return risposteViewModel;
        }
    }
}