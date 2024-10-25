using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class RuoloViewModel
    {
        public string NomeRuolo {get; set;}
        
        public static RuoloViewModel FromDataRow(DataRow dataRow)
        {
            var ruoloViewModel = new RuoloViewModel 
            {
            NomeRuolo = Convert.ToString(dataRow["Nome"])
            };

            return ruoloViewModel;  
        }
    }
}