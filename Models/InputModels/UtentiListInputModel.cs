using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Customizations.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Models.InputModels
{
    [ModelBinder(BinderType = typeof(UtentiListInputModelBinder))]  
    public class UtentiListInputModel
    {
        public int Id { get;}
        public string Nome { get;}
        public string Cognome {get;}
        public string Email { get;}       
        public string Password { get;}
        public DateTime DataDiNascita { get;}
        public string Genere {get;}
        public string NumeroDiTelefono {get;}
        public string Ruolo {get;}
        public UtentiListInputModel(int id, string nome, string cognome, string email, DateTime dataDiNascita, string password, string genere, string numeroDiTelefono, string ruolo)
        {
            this.Id = id;
            this.Nome=nome;
            this.Cognome = cognome;
            this.Email=email;
            this.DataDiNascita=dataDiNascita;
            this.Password=password;
            this.Genere = genere;
            this.NumeroDiTelefono = numeroDiTelefono;
            this.Ruolo = ruolo;
            
        } 
    }
        
}