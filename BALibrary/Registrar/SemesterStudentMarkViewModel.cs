using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SemesterStudentMarkViewModel
    {
        public int AcademicYearId { get; set; }//from form
        public int AcademicYearSectionId { get; set; }//from form
        public int StudentPromotionId { get; set; }
        public decimal GrandTotalMark { get; set; } //from one semester all subjects       
        public decimal GrandAverage { get; set; } //from one semester all subjects
        public decimal GrandRank { get; set; } //from one semester all subjects
        public ICollection<StudentMarkViewModel> StudentMarks { get; set; }
    }
}
