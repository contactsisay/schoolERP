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
    public class StudentAbsentee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Student")]
        public int StudentPromotionId { get; set; }
        [Display(Name ="Attendance Taken By")]
        public int SectionTeacherSubjectId { get; set; }
        [Display(Name ="Absent Date")]
        public DateTime AbsentDate { get; set; }
        public string? FilePath { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Reason { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreateAt { get; set; }
        [ScaffoldColumn(false)]
        public int CreatedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? ApprovedAt { get; set; }
        [ScaffoldColumn(false)]
        public int? ApprovedBy { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("StudentPromotionId")]
        public virtual StudentPromotion StudentPromotion { get; set; }
        [ForeignKey("SectionTeacherSubjectId")]
        public virtual SectionTeacherSubject SectionTeacherSubject { get; set; }
    }
}
