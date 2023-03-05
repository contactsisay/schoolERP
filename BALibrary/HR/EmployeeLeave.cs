using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeLeave
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Leave Taken Date")]
        public DateTime LeaveTakenDate { get; set; }

        [Display(Name = "Leave Days")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LeaveDays { get; set; }

        [Display(Name = "Rest Days")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RestDays { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
