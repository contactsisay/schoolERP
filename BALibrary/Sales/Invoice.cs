using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Sales
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Invoice Type")]
        public int InvoiceTypeId { get; set; }
        [Required]
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
        [Required]
        [Display(Name = "Invoice Total")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InvoiceTotal { get; set; }
        [Required]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        [Display(Name = "Package")]
        [ScaffoldColumn(false)]
        public int? PackageId { get; set; }

        [Required]
        [Display(Name = "Created By")]
        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        [ForeignKey("InvoiceTypeId")]
        public virtual InvoiceType? InvoiceType { get; set; }
    }
}
