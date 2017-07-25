using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeMap.Models.CollegeMapModels
{
    public class DegreeType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IList<College> Colleges { get; set; }
    }
}
