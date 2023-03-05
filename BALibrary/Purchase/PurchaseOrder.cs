using BALibrary.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Purchase
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Supplier Invoice No")]
        public string? SupplierInvoiceNo { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        [Required]
        [Display(Name = "Requested Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequiredAmount { get; set; }
        [Required]
        [Display(Name = "Approved Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ApprovedAmount { get; set; }

        [ScaffoldColumn(false)]
        public int RequestedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime RequestedAt { get; set; }
        [ScaffoldColumn(false)]
        public int? ApprovedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? ApprovedAt { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }//passed to stock or not
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }
    }
}
