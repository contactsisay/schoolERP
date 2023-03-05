using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Examination
{
    public class ExamGroupDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Exam Group")]
        public int ExamGroupId { get; set; }
        [Required]
        [Display(Name ="Exam")]
        public int ExamId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Passing Mark")]
        public decimal PassingMark { get; set; } = 50;

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }
        [ForeignKey("ExamGroupId")]
        public virtual ExamGroup? ExamGroup { get; set; }
    }
}
