using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class EmployeeSocialMediaLink
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Social Media")]
        public int SocialMediaId { get; set; }
        [Required]
        [Display(Name = "Link Address")]
        public string LinkAddress { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
        [ForeignKey("SocialMediaId")]
        public virtual SocialMedia? SocialMedia { get; set; }
    }
}
