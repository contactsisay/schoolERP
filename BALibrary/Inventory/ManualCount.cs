using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Inventory
{
    public class ManualCount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Batch")]
        public int ProductBatchId { get; set; }
        [Required]
        [Display(Name = "Count Value")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountValue { get; set; }
        [Required]
        [Display(Name = "Counted Date")]
        public DateTime CountedDate { get; set; }
        [Required]
        [Display(Name = "Counted By")]
        [ScaffoldColumn(false)]
        public int EmployeeId { get; set; }//counted by

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("ProductBatchId")]
        public virtual ProductBatch? ProductBatch { get; set; }
    }
}
