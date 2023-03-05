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
    public class TimeTable
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Academic Year Section")]
        public int AcademicYearSectionId { get; set; } //SectionId and ClassId-> from academic year and session
        
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("AcademicYearSectionId")]
        public virtual AcademicYearSection? AcademicYearSection { get; set; }
    }
}
