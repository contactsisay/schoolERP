using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeTransfer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "From Position")]
        public int FromJobPositionId { get; set; }
        [Required]
        [Display(Name = "To Position")]
        public int ToJobPositionId { get; set; }
        [Required]
        [Display(Name = "From Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FromBasicSalary { get; set; }
        [Required]
        [Display(Name = "To Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToBasicSalary { get; set; }
        [Required]
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
