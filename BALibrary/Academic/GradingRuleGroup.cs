using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary.Academic
{
    public class GradingRuleGroup
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } //e.g. KG Grading, Elementary Grading, High School Grading, etc

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
