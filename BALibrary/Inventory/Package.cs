using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }
        [Required]
        [Display(Name = "Is Percentage?")]
        public bool IsPercentage { get; set; } = true;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [ScaffoldColumn(false)]
        public decimal TotalPrice { get; set; }
        [ScaffoldColumn(false)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CalculatedPrice { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Total Items")]
        public int TotalItems { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
