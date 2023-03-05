using BALibrary.Academic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class StudentPromotion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Student")]
        public int StudentId { get; set; }
        [Required]
        [Display(Name = "Academic Year Section")]
        [ScaffoldColumn(false)]
        public int AcademicYearSectionId { get; set; }//current section -> back to semester, year, section, etc
        [Required]
        [Display(Name ="Promoted Date")]
        public DateTime PromotedDate { get; set; }
        [Required]
        [Display(Name ="Promoted By")]
        public int PromotedBy { get; set; }
        [Required]
        public bool IsClassChange { get; set; } //to check if promoted to another class

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        [ForeignKey("AcademicYearSectionId")]
        public virtual AcademicYearSection? AcademicYearSection { get; set; }
    }
}
