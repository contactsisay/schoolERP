using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Registrar
{
    public class StudentGuardian
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }
        [Required]
        [Display(Name ="Relationship Type")]
        public int RelationshipId { get; set; }
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        [Required]
        [Display(Name ="Mobile No")]
        public string MobileNo { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage ="Please enter valid email address")]
        public string EmailAddress { get; set; }
        public string Occupation { get; set; }
        public string PhotoPath { get; set; }
        [Required]
        public string Password { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        [ForeignKey("RelationshipId")]
        public virtual Relationship? Relationship { get; set; }
    }
}
