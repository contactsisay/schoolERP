using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Registrar
{
    public class SubjectExamMarkViewModel
    {
        public int ExamGroupDetailId { get; set; }
        public string ExamName { get; set; }
        public decimal ExamWeight { get; set; }
        public decimal ExamMark { get; set; }
    }
}
