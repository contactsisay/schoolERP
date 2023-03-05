using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeSalary
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Is Appraisal?")]
        public bool IsAppraisal { get; set; } = true;
        [Required]
        [Display(Name = "Updated Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UpdatedAmount { get; set; }
        [Required]
        [Display(Name = "Is Percentage?")]
        public bool IsPercentage { get; set; } = true;
        [Required]
        [Display(Name = "From Basic Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FromBasicSalary { get; set; } //including updated one
        [Required]
        [Display(Name = "To Basic Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToBasicSalary { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
