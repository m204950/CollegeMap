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

        // [Required]
        // [Range(10, 100000)]
        [Display(Name = "Max Travel Dist. (mi)")]
        public int? MaxTravel { get; set; }

        [Required]
        [Display(Name = "Min Enrollment")]
        public int MinimumEnrollment { get; set; }

        [Display(Name = "College Name Contains")]
        public string NameContains { get; set; }

        [Display(Name = "Address Contains")]
        public string AddressContains { get; set; }

        [Required]
        [Display(Name = "Max Enrollment")]
        public int MaximumEnrollment { get; set; }

        [Required]
        [Display(Name = "Max Average Net Total Cost")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Range(300, 1000000)]
        public int MaxTotalCost { get; set; }

        [Required]
        [Display(Name = "College Type Desired (multi-select)")]
        public IEnumerable<int> CollegeTypeIDs { get; set; }

        [Required]
        [Display(Name = "States to search (multi-select)")]
        public IEnumerable<string> StateIDs { get; set; }

        [Required]
        [Display(Name = "Highest Degree Level Needed")]
        public int DegreeTypeID { get; set; }

        public List<College> Colleges { get; set; }

        public float HomeLatitude { get; set; }

        public float HomeLongitude { get; set; }

        public string CollegeDataProvider { get; set; }

        public string CollegeDataVersion { get; set; }

        public string DistanceMessage { get; set; }

        public List<SelectListItem> CollegeTypes { get; set; }

        public List<SelectListItem> DegreeTypes { get; set; }

        public List<SelectListItem> States { get; set; }

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

        public void CreateStateSelects(IEnumerable<string> states)
        {
            States = new List<SelectListItem>();

            foreach (string state in states)
            {
                States.Add(new SelectListItem
                {
                    Value = state,
                    Text = state,
                    Selected = true
                });
            }

        }

        public QueryCollegeViewModel()
        {
        }

        public QueryCollegeViewModel(IEnumerable<CollegeType> collegeTypes, IEnumerable<DegreeType> degreeTypes,
            IEnumerable<string> states)
        {

            CreateCollegeTypeSelects(collegeTypes);
            CreateDegreeTypeSelects(degreeTypes);
            CreateStateSelects(states);
        }
    }
}
