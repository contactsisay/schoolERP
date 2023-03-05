using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Sales
{
    public class InvoiceDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Invoice")]
        public int InvoiceId { get; set; }
        [Required]
        [Display(Name = "Product Batch")]
        public int ProductBatchId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Selling Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; } //get from product batch
        [Required]
        [Display(Name = "Row Total")]
        [Column(TypeName = "decimal(18,2)")]
        [ScaffoldColumn(false)]
        public decimal RowTotal { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
