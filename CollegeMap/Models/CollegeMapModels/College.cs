using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeMap.Models.CollegeMapModels
{
    public class College
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }

        // Associate, Bachelors, Masters, Doctorate
        [Display(Name = "Highest Degree")]
        public DegreeType HighestDegreeOffered { get; set; }
        // Total enrollment
        public int Enrollment { get; set; }
        // public private, for-profit
        public CollegeType Type { get; set; }
        [Display(Name = "Tuition")]
        public int AnnualTuition { get; set; }
        // zero if not available.  view to display N/A when zero
        [Display(Name = "Annual R & B")]
        public int AnnualRoomAndBoard { get; set; }
        public string Website { get; set; }
        // Address can be anything google maps can handle
        public string Address { get; set; }
        // distance used at query time, based on home location used in the query
        public int Distance { get; set; }

    }
}
