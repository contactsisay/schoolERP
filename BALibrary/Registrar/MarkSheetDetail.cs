using BALibrary.Academic;
using BALibrary.Examination;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Registrar
{
    public class MarkSheetDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Mark Sheet")]
        public int MarkSheetId { get; set; }
        [Required]
        [Display(Name = "Exam")]
        public int ExamGroupDetailId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Exam Mark")]
        public decimal ExamMark { get; set; }        
        [Required]
        [Display(Name = "Is Passed")]
        public bool IsPassed { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Rank From Section")]
        public int? RarkFromSection { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Rank From Grade")]
        public int? RarkFromGrade { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("MarkSheetId")]
        public virtual MarkSheet? MarkSheet { get; set; }
    }
}
