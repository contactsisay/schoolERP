using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class StudentMarkViewModel
    {
        public int AcademicYearId { get; set; }//from form
        public int AcademicYearSectionId { get; set; }//from form
        public int StudentPromotionId { get; set; }
        public string IDNo { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string SectionName { get; set; }
        public decimal OverallMark { get; set; } //from all subjects        
        public int RankFromSection { get; set; } //with all subjects
        public int RankFromGrade { get; set; } //with all subjects
        public ICollection<SubjectMarkViewModel> SubjectMarks { get; set; }
    }
}
