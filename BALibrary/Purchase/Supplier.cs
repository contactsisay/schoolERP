using System.ComponentModel.DataAnnotations;

namespace BALibrary.Purchase
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Supplier Phone")]
        public string SupplierPhone { get; set; }
        [Required]
        [Display(Name = "TIN No")]
        public string TINNo { get; set; }
        [Display(Name = "VAT Reg. No")]
        public string VATRegNo { get; set; }
        [Required]
        [Display(Name = "Contact Person Name")]
        public string ContactPersonName { get; set; }
        [Display(Name = "Contact Person Email")]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string ContactPersonEmail { get; set; }
        [Required]
        [Display(Name = "Contact Person Mobile")]
        public string ContactPersonMobile { get; set; }
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
