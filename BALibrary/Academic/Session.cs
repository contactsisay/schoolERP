using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Academic
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "School Type")]
        public string SchoolType { get; set; }//from enum (KG, Primary, Secondary, Highschool, etc)
        [Required]
        public int Year { get; set; } //current year        
        [Required]
        [Display(Name = "Grading Rule Group")]
        public int GradingRuleGroupId { get; set; }

        [Display(Name = "Min. Student")]
        public int MinStudent { get; set; }
        [Display(Name = "Max. Student")]
        public int MaxStudent { get; set; }
        [Display(Name = "Opening Date")]
        public DateTime OpeningDate { get; set; }
        [Display(Name = "Closing Date")]
        public DateTime ClosingDate { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("GradingRuleGroupId")]
        public virtual GradingRuleGroup? GradingRuleGroup { get; set; }
    }
}
