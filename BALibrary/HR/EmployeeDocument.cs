using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeDocument
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Document Type")]
        public int DocumentTypeId { get; set; }
        [Required]
        [Display(Name = "Document Title")]
        public string DocumentTitle { get; set; }
        [Required]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType? DocumentType { get; set; }
    }
}
