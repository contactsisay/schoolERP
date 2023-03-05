using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeOffday
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Off Day")]
        public string OffDay { get; set; } //Monday, Tuesday, Wednsday, Thursday, Friday, Saturday or Sunday
        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
