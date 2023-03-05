using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class TimeTableDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Time Table")]
        public int TimeTableId { get; set; }
        [Display(Name ="Teacher")]
        public int EmployeeId { get; set; } //teacher id
        [Required]
        [Display(Name ="Period")]
        public int PeriodId { get; set; }
        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; } //from class subject-group
        [Required]
        [Display(Name ="Week Day")]
        public string WeekDay { get; set; }//Monday, Tuesday, etc
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("TimeTableId")]
        public virtual TimeTable? TimeTable { get; set; }
        [ForeignKey("PeriodId")]
        public virtual Period? Period { get; set; }
    }
}
