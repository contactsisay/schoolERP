using BALibrary.Academic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Registrar
{
    public class MarkSheet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Student Promotion")]
        public int StudentPromotionId { get; set; } //from student promotions (current student session)
        [Required]
        [Display(Name = "Subject")]
        public int SubjectGroupDetailId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Average { get; set; } //average from all exams
        [Display(Name ="Registered By")]
        public int RegisteredBy { get; set; }
        [Required]
        [Display(Name = "Registered At")]
        public DateTime RegisteredAt { get; set; }
        [Display(Name ="Modified By")]
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Display(Name ="Approved By")]
        public int? ApprovedBy { get; set; }
        [Display(Name ="Approved At")]
        public DateTime? ApprovedAt { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("StudentPromotionId")]
        public virtual StudentPromotion? StudentPromotion { get; set; }
    }
}
