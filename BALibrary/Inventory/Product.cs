using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Product Category")]
        public int ProductCategoryId { get; set; }
        [Required]
        [Display(Name = "Unit of Measurement")]
        public int UomId { get; set; }
        [Required]
        [Display(Name = "Min. Order Level")]
        public int MinimumOrderLevel { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory? ProductCategory { get; set; }
        [ForeignKey("UomId")]
        public virtual Uom? Uom { get; set; }
    }
}
