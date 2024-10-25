using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.ViewModels
{
    public class CourseDetailProgressViewModel
    {
        public CourseDetailViewModel Dettagli {get;set;}
        public ProgressiViewModel Progressi {get;set;} 

        public CourseDetailProgressViewModel(){
            Dettagli = new CourseDetailViewModel();
            Progressi = new ProgressiViewModel();

        }
        
    }
}