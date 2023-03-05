using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class ProductBatch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Registed By")]
        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }//registered by
        [Required]
        [Display(Name = "Batch No")]
        public string BatchNo { get; set; }
        [Display(Name = "Best Before")]
        public DateTime? BestBefore { get; set; }
        [Required]
        [Display(Name = "Manufactured Date")]
        public DateTime ManufacturedDate { get; set; }
        [Required]
        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }
        [Required]
        [Display(Name = "Purchasing Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasingPrice { get; set; }
        [Required]
        [Display(Name = "Selling Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }
        [Required()]
        [Display(Name = "Is Taxable?")]
        public bool IsTaxable { get; set; } = true;
        [Required()]
        [Display(Name = "Is Sellable?")]
        public bool IsSellable { get; set; } = true;

        [ScaffoldColumn(false)]
        public int PurchaseOrderId { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
