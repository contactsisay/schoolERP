using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeShift
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Shift Type")]
        public int ShiftTypeId { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ShiftTypeId")]
        public virtual ShiftType? ShiftType { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
