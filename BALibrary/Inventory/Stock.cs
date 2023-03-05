using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Batch")]
        public int ProductBatchId { get; set; }
        [Required]
        [Display(Name = "Initial Quantity")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialQuantity { get; set; }
        [Required]
        [Display(Name = "Sold Quantity")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoldQuantity { get; set; }
        [Required]
        [Display(Name = "Current Quantity")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentQuantity { get; set; }
        [Required]
        [Display(Name = "Action Taken")]
        public int ActionTaken { get; set; } //sold, purchased or expired (wasted)
        [Required]
        [Display(Name = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
        [Required]
        [Display(Name = "Updated By")]
        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }//updated by

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ProductBatchId")]
        public virtual ProductBatch? ProductBatch { get; set; }
    }
}
