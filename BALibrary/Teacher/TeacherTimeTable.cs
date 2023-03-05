using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Teacher
{
    public class TeacherTimeTable
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
