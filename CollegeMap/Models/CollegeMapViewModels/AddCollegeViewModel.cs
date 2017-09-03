using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CollegeMap.Models.CollegeMapModels;

namespace CollegeMap.Models.CollegeMapViewModels
{
    public class AddCollegeViewModel
    {
        [Required]
        [Display(Name = "College Name")]
        public string Name { get; set; }

        [Display(Name = "Notes")]
        public string Description { get; set; }

        [Required]
        public int Enrollment { get; set; }

        [Required]
        [Display(Name = "Tuition - In State")]
        public int AnnualTuition { get; set; }

        [Required]
        [Display(Name = "Tuition - Out of State")]
        public int AnnualTuitionOut { get; set; }

        [Required]
        [Display(Name = "Average Annual Net Cost")]
        public int AvgNetPrice { get; set; }

        [Url]
        public string Website { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "College Type")]
        public int CollegeTypeID { get; set; }

        [Required]
        [Display(Name = "Highest Degree Offered")]
        public int DegreeTypeID { get; set; }

        [Required]
        public float AcceptanceRate { get; set; }

        // Used for edit
        public int ID { get; set; }

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
                    Text = collegeType.Name
                });
            }

        }
        public AddCollegeViewModel()
        {
        }

        public AddCollegeViewModel(IEnumerable<CollegeType> collegeTypes, IEnumerable<DegreeType> degreeTypes)
        {

            CreateCollegeTypeSelects(collegeTypes);
            CreateDegreeTypeSelects(degreeTypes);
        }
    }
}
