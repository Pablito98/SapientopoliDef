using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Models.ViewModels
{
    public class UtentiListViewModel
    {
        public List<UtentiViewModel> Utenti {get;set;}
        public UtentiListInputModel Input {get;set;}
        public SearchListInputModel Ricerca {get;set;}
    }
}