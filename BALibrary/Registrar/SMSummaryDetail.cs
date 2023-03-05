using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SMSummaryDetail
    {
        [Key]
        public int Id { get; set; }
        public int SMSummaryId { get; set; }
        public int SubjectGroupDetailId { get; set; }
        [Display(Name ="Subject Total")]
        [Column(TypeName ="decimal(18,2)")]
        public decimal SubjectTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Average { get; set; }
        [Display(Name ="Rank From Section")]
        public int RankFromSection { get; set; }
        [Display(Name = "Rank From Grade")]
        public int RankFromGrade { get; set; }
        public bool IsPassed { get; set; }
        public string Remark { get; set; }
    }
}
