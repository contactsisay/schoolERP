using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SectionTeacherSubject
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Academic Year Section")]
        [Required]
        public int AcademicYearSectionId { get; set; }
        [Required]
        [Display(Name ="Subject")]
        public int SubjectGroupDetailId { get; set; }
        [Required]
        [Display(Name ="Teacher")]
        public int TeacherId { get; set; }
        [Required]
        [Display(Name = "Total Class Per Week")]
        public int TotalClassPerWeek { get; set; }
        [Required]
        [Display(Name ="Assigned Date")]
        public DateTime AssignedDate { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
