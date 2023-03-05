using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Academic
{
    public class AcademicYearSection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; }
        [Required]
        [Display(Name = "Section")]
        public int SectionId { get; set; }        
        [Required]
        [Display(Name = "No of Student")]
        public int NoOfStudent { get; set; }
        [Display(Name = "Section Representative")]
        public int? StudentId { get; set; } //representative
        [Display(Name = "Room")]
        public int? RoomId { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("SectionId")]
        public virtual Section? Section { get; set; }
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear? AcademicYear { get; set; }
    }
}
