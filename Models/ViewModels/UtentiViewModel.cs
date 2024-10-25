using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class UtentiViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }
 
        public string Cognome {get;set;}

        public string Email { get; set; }       

        public string Password { get; set; }

        public DateTime DataDiNascita { get; set; }

        public string Genere {get;set;}

        public string NumeroDiTelefono {get;set;}

        public string Ruolo {get;set;}

        public static UtentiViewModel FromDataRow(DataRow utentiRow)
        {
            
            var utentiViewModel = new UtentiViewModel
            {
                Id = Convert.ToInt32(utentiRow["id"]),
                Nome = Convert.ToString(utentiRow["nome"]),
                Email = Convert.ToString(utentiRow["email"]),                
                Password = Convert.ToString(utentiRow["password"]),
                Cognome = Convert.ToString(utentiRow["cognome"]),
                Genere = Convert.ToString(utentiRow["genere"]),
                NumeroDiTelefono = Convert.ToString(utentiRow["numero_telefono"]),
                DataDiNascita = Convert.ToDateTime(utentiRow["data_di_nascita"]),
                Ruolo = Convert.ToString(utentiRow["Ruolo"])
            };
            return utentiViewModel;
        }
    }
}