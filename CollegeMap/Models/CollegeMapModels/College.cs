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
        // maximum number of entries to use google api to compute driving distance
        public const int MAX_DRIVE_DIST_ENTRIES = 15;

        public string Name { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }

        // CollegeScorecard.com ID
        [Display(Name = "CollegeScorecard.com ID")]
        public int CollegeScorecardID { get; set; }

        // Non-Degree, Certificat, Associate, Bachelors, Graduate
        [Display(Name = "Highest Degree")]
        public DegreeType HighestDegreeOffered { get; set; }
        // Total enrollment
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int Enrollment { get; set; }
        // public private, for-profit
        public CollegeType Type { get; set; }

        [Display(Name = "Tuition - In State")]
        [DisplayFormat(DataFormatString = "{0:c0}")]
        public int AnnualTuition { get; set; }

        [Display(Name = "Tuition - Out of State")]
        [DisplayFormat(DataFormatString = "{0:c0}")]
        public int AnnualTuitionOut { get; set; }

        [Display(Name = "Acceptance Rate")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public float AcceptanceRate { get; set; }

        // The average annual total cost of attendance, including tuition and fees, 
        // books and supplies, and living expenses, minus the average grant/scholarship aid. 
        [Display(Name = "Average Annual Net Cost of Attendance")]
        [DisplayFormat(DataFormatString = "{0:c0}")]
        public int AvgNetPrice { get; set; }

        public string State { get; set; }
        public string Website { get; set; }
        // Address can be anything google maps can handle
        public string Address { get; set; }
        // distance used at query time, based on home location used in the query
        public int Distance { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [Display(Name = "Graduation Rate")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public float GraduationRate { get; set; }

        [Display(Name = "Percentage of White Students")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public float PercentWhite { get; set; }

    }
}
