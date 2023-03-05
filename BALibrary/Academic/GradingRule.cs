using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BALibrary.Academic
{
    public class GradingRule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Mark From")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarkFrom { get; set; } = 0;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Middle { get; set; } = 0;
        [Required]
        [Display(Name = "Mark To")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarkTo { get; set; } = 0;

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
