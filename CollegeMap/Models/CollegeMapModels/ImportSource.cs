using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeMap.Models.CollegeMapModels
{
    public class ImportSource
    {
        public string Source { get; set; }
        public string Version { get; set; }
        public DateTime ImportTime { get; set; }
        public int ID { get; set; }
    }
}
