using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Examination
{
    public class ExamGroup
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Maximum Mark")]
        public decimal MaximumMark { get; set; } = 100;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Passing Mark")]
        public decimal PassingMark { get; set; } = 50;
        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
