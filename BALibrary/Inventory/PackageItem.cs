using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class PackageItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Package")]
        public int PackageId { get; set; }
        [Required]
        [Display(Name = "Product Batch")]
        public int ProductBatchId { get; set; }
        [Required]
        public int Quantity { get; set; } = 1;
        [ScaffoldColumn(false)]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("PackageId")]
        public virtual Package? Package { get; set; }
        [ForeignKey("ProductBatchId")]
        public virtual ProductBatch? ProductBatch { get; set; }
    }
}
