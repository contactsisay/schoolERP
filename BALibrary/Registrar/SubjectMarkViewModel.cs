using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SubjectMarkViewModel
    {
        public int StudentPromotionId { get; set; }
        public int SubjectGroupDetailId { get; set; }
        public decimal TotalMark { get; set; } //subject-exams-mark 
        public int RankFromSection { get; set; } //with the single subject
        public int RankFromGrade { get; set; } //with the single subject
        public ICollection<SubjectExamMarkViewModel> ExamMarks { get; set; }
    }
}
