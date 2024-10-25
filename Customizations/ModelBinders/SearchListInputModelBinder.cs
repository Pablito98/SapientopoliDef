using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.InputModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Sapientopoli.Customizations.ModelBinders
{
    public class SearchListInputModelBinder : IModelBinder
    {
        public SearchListInputModelBinder()
        {
        }
        
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            
            //Recuperiamo i valori grazie ai value provider
            string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
 
            //Creiamo l'istanza del CourseListInputModel
            var inputModel = new SearchListInputModel(search);

            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result = ModelBindingResult.Success(inputModel);

            //Restituiamo un task completato
            return Task.CompletedTask;
        }
    }
}