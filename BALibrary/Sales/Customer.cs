using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Sales
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Customer Type")]
        public int CustomerTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string TINNo { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("CustomerTypeId")]
        public virtual CustomerType? CustomerType { get; set; }
    }
}
