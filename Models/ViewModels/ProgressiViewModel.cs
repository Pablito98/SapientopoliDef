using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class ProgressiViewModel
    {
        public CourseViewModel Corso { get; set; }
        public UtentiViewModel Utente { get; set; }
        public int IdCorso {get; set;}
        public int IdUtente{get;set;}
        public double Progresso {get; set;}
        public int LezioniCompletate {get; set;}

        public static ProgressiViewModel FromDataRow(DataRow dataRow)
        {
            var progressiViewModel = new ProgressiViewModel {
                IdCorso = Convert.ToInt32(dataRow["id_corso"]),
                IdUtente = Convert.ToInt32(dataRow["id_utente"]), 
                Progresso = Convert.ToDouble(dataRow["progresso"]),
                LezioniCompletate = Convert.ToInt32(dataRow["lezioni_completate"])
            };

            return progressiViewModel;
        }
    }
}