using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }

        [Required]
        [Display(Name = "Is Absent?")]
        public bool IsAbsent { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
