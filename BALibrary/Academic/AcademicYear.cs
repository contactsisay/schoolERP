using BALibrary.Examination;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Academic
{
    public class AcademicYear
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Session")]
        public int SessionId { get; set; }
        [Required]
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }
        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }
        [Required]
        [Display(Name = "Subject Group")]
        public int SubjectGroupId { get; set; }
        [Required]
        [Display(Name = "Exam Group")]
        public int ExamGroupId { get; set; }
        [Display(Name = "Class Representative")]
        public int? StudentId { get; set; } //representative
        [Display(Name = "Home Room Teacher")]
        public int? EmployeeId { get; set; } //Home room teacher

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session? Session { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester? Semester { get; set; }
        [ForeignKey("SubjectGroupId")]
        public virtual SubjectGroup? SubjectGroup { get; set; }
        [ForeignKey("ExamGroupId")]
        public virtual ExamGroup? ExamGroup { get; set; }
    }
}
