using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class JobPosition
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Initial Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialSalary { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
    }
}
