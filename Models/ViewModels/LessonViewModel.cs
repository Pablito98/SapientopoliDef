using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class LessonViewModel
    {
        public int Id{get; set;}
        public int CorsoId{get; set;}
        public string Titolo {get; set;}
        public TimeSpan Durata {get; set;}  
        public string Descrizione {get; set;}
        public string Teoria {get; set;}
        public string Video {get; set;}
        public string Immagine {get; set;}
        public int Sequenza {get; set;}
        public bool IsPrimaLezione { get; set; }
        public bool IsLezionePrecedenteCompletata { get; set; }

        public static LessonViewModel FromDataRow(DataRow dataRow)
        {
            var lessonViewModel = new LessonViewModel {
                Id = Convert.ToInt32(dataRow["Id"]),
                CorsoId = Convert.ToInt32(dataRow["corso_id"]),
                Titolo = Convert.ToString(dataRow["titolo"]),
                Durata = TimeSpan.Parse(Convert.ToString(dataRow["durata"])),
                Descrizione = Convert.ToString(dataRow["descrizione"]),
                Teoria = Convert.ToString(dataRow["teoria"]), 
                Video = Convert.ToString(dataRow["video"]),
                Immagine = Convert.ToString(dataRow["immagine"]),
                Sequenza = Convert.ToInt32(dataRow["sequenza"])
            };

            return lessonViewModel;
        }
    }
}
