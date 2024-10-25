using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    //classe che rappresenta l'entità CORSO
    public class CourseViewModel
    {
        public int Id {get; set;}
        public string NomeCorso {get; set;}
        public string ImagePath {get; set;}
        public string Descrizione {get; set;}
        public double Rating {get; set;}
        public double? Progresso{get; set;}

        public static CourseViewModel FromDataRow(DataRow courseRow)
        {
            var courseViewModel = new CourseViewModel
            {
                NomeCorso= Convert.ToString(courseRow["nome_corso"]), //recupero il titolo dal db leggendo la colonna title 
                ImagePath = Convert.ToString(courseRow["imagePath"]),
                Descrizione = Convert.ToString(courseRow["descrizione"]),
                Rating = Convert.ToDouble(courseRow["rating"]),
                Id = Convert.ToInt32(courseRow["Id"]),
                Progresso = courseRow.Table.Columns.Contains("progresso") && !Convert.IsDBNull(courseRow["progresso"]) ? (double?)Convert.ToDouble(courseRow["progresso"]) : null,
            };
            return courseViewModel;
 
        }
    }
}