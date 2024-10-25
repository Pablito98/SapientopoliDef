using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sapientopoli.Models.ViewModels
{
    
    public class LoginViewModel
    {
        public LoginViewModel(string nome, string email, int id, string password)
        {
            this.Nome = nome;
            this.Email = email;
            this.Id = id;
            this.Password = password; 

        }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
    }

}
