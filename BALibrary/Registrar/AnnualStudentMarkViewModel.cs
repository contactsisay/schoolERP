using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class AnnualStudentMarkViewModel
    {
        public int AcademicYearId { get; set; }//from form
        public int AcademicYearSectionId { get; set; }//from form
        public int StudentPromotionId { get; set; }   
        public decimal GrandTotalMark { get; set; } //from two semesters        
        public decimal GrandAverage { get; set; } //from two semesters
        public decimal GrandRank { get; set; } //from two semesters

        public ICollection<SemesterStudentMarkViewModel> SemesterStudentMarks { get; set; }
    }
}
