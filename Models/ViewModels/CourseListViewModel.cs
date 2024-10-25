using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.InputModels;

namespace Sapientopoli.Models.ViewModels
{
    public class CourseListViewModel
    {
        public List<CourseViewModel> Courses {get;set;} 
        public SearchListInputModel Ricerca {get;set;}
     
    }
}