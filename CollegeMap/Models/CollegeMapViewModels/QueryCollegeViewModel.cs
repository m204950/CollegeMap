using CollegeMap.Models.CollegeMapModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeMap.Models.CollegeMapViewModels
{
    public class QueryCollegeViewModel
    {
        [Required]
        [Display(Name = "Home Address")]
        public string HomeAddress { get; set; }

        [Required]
        [Display(Name = "Max Travel Distance (miles)")]
        public int MaxTravel { get; set; }

        [Required]
        [Display(Name = "Min Enrollment")]
        [Range(300, 100000)]
        public int MinimumEnrollment { get; set; }

        [Display(Name = "College Name Contains")]
        public string NameContains { get; set; }

        [Required]
        [Display(Name = "Max Enrollment")]
        [Range(300, 100000)]
        public int MaximumEnrollment { get; set; }

        [Required]
        [Display(Name = "Max Tuition plus R&B")]
        [Range(300, 1000000)]
        public int MaxTotalCost { get; set; }

        [Required]
        [Display(Name = "College Type Desired (multi-select)")]
        public IEnumerable<int> CollegeTypeIDs { get; set; }

        [Required]
        [Display(Name = "Highest Degree Level Neede")]
        public int DegreeTypeID { get; set; }

        public List<College> Colleges { get; set; }

        public List<SelectListItem> CollegeTypes { get; set; }

        public List<SelectListItem> DegreeTypes { get; set; }

        public void CreateDegreeTypeSelects(IEnumerable<DegreeType> degreeTypes)
        {
            DegreeTypes = new List<SelectListItem>();

            foreach (DegreeType degreeType in degreeTypes)
            {
                DegreeTypes.Add(new SelectListItem
                {
                    Value = degreeType.ID.ToString(),
                    Text = degreeType.Name
                });
            }

        }
        public void CreateCollegeTypeSelects(IEnumerable<CollegeType> collegeTypes)
        {
            CollegeTypes = new List<SelectListItem>();

            foreach (CollegeType collegeType in collegeTypes)
            {
                CollegeTypes.Add(new SelectListItem
                {
                    Value = collegeType.ID.ToString(),
                    Text = collegeType.Name,
                    Selected = true
                });
            }

        }
        public QueryCollegeViewModel()
        {
        }

        public QueryCollegeViewModel(IEnumerable<CollegeType> collegeTypes, IEnumerable<DegreeType> degreeTypes)
        {

            CreateCollegeTypeSelects(collegeTypes);
            CreateDegreeTypeSelects(degreeTypes);
        }
    }
}
