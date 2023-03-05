using BALibrary.Registrar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Teacher
{
    public class StudentDisciplinaryAction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Student")]
        public int StudentPromotionId { get; set; }
        [Display(Name = "Class Teacher")]
        public int SectionTeacherSubjectId { get; set; }
        [Display(Name ="Action Taken By")]
        public int? TeacherId { get; set; }
        [Required]
        [Display(Name ="Action Type")]
        public int ActionTypeId { get; set; }
        public string? FilePath { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ActionTypeId")]
        public virtual ActionType ActionType { get; set; }
        [ForeignKey("StudentPromotionId")]
        public virtual StudentPromotion StudentPromotion { get; set; }
        [ForeignKey("SectionTeacherSubjectId")]
        public virtual SectionTeacherSubject SectionTeacherSubject { get; set; }
    }
}
