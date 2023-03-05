using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SMSummary
    {
        [Key]
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int AcademicYearSectionId { get; set; }
        public int StudentPromotionId { get; set; }
        public string IDNo { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string SectionName { get; set; }
        [Display(Name ="Out Of (%)")]
        public int OutOf { get; set; }
        [Display(Name ="Total Mark")]
        [Column(TypeName ="decimal(18,2)")]
        public decimal TotalMark { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Average { get; set; }
        [Display(Name ="Rank From Section")]
        public int RankFromSection { get; set; }
        [Display(Name = "Rank From Grade")]
        public int RankFromGrade { get; set; }
        [Display(Name = "Rank From School")]
        public int RankFromSchool { get; set; }
        public bool IsPassed { get; set; }
        public string Remark { get; set; }
    }
}
