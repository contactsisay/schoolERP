using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BALibrary.Academic
{
    public class GradingRuleGroupDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Grading Rule Group")]
        public int GradingRuleGroupId { get; set; }
        [Required]
        [Display(Name = "Grading Rule")]
        public int GradingRuleId { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("GradingRuleGroupId")]
        public virtual GradingRuleGroup? GradingRuleGroup { get; set; }
        [ForeignKey("GradingRuleId")]
        public virtual GradingRule? GradingRule { get; set; }
    }
}
