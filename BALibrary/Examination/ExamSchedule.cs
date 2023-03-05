using BALibrary.Academic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Examination
{
    public class ExamSchedule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Academic Year")]
        public int AcademicYearId { get; set; } //plan per class
        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; } //from subject group  
        [Required]
        [Display(Name = "Exam")]
        public int ExamId { get; set; } //from exam group
        [Required]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Duration { get; set; }
        [Required]
        [Display(Name ="Room")]
        public int RoomId { get; set; }
        [Required]
        public bool Publish { get; set; } = false;
        [Required]
        [Display(Name = "Publish Result")]
        public bool PublishResult { get; set; } = false;

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear? AcademicYear { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }
        public virtual Room? Room { get; set; }
    }
}
