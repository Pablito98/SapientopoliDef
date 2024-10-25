using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.InputModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Sapientopoli.Customizations.ModelBinders
{
    public class UtentiListInputModelBinder : IModelBinder
    {
        public UtentiListInputModelBinder()
        {

        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Recuperiamo i valori grazie ai value provider
            int id;  
            string nome = bindingContext.ValueProvider.GetValue("Nome").FirstValue;
            string cognome = bindingContext.ValueProvider.GetValue("Cognome").FirstValue;
            string email = bindingContext.ValueProvider.GetValue("Email").FirstValue;
            DateTime dataDiNascita = new DateTime(); 
            string password = bindingContext.ValueProvider.GetValue("Password").FirstValue;  
            string genere = bindingContext.ValueProvider.GetValue("Genere").FirstValue;
            string numeroDiTelefono = bindingContext.ValueProvider.GetValue("NumeroDiTelefono").FirstValue;
            string ruolo = bindingContext.ValueProvider.GetValue("Ruolo").FirstValue;
            try
            {
                dataDiNascita = Convert.ToDateTime(bindingContext.ValueProvider.GetValue("DataDiNascita").FirstValue);
                id = Convert.ToInt32(bindingContext.ValueProvider.GetValue("Id").FirstValue);
            }
            catch(System.FormatException)
            {
                id = 0; // or other default value as appropriate in context.
            }        
            //int id = Convert.ToInt32(bindingContext.ValueProvider.GetValue("Id").FirstValue);
 
            //Creiamo l'istanza del CourseListInputModel
            var inputModel = new UtentiListInputModel(id,nome,cognome,email,dataDiNascita,password,genere,numeroDiTelefono,ruolo);
            
            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result = ModelBindingResult.Success(inputModel);

            //Restituiamo un task completato
            return Task.CompletedTask;
        }
        
    }
}