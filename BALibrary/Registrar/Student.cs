using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BALibrary.Academic;
using BALibrary.Admin;

namespace BALibrary.Registrar
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "ID No")]
        public string IDNo { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Father Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Grandfather Name")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Display(Name ="DOB")]
        public DateTime Dob { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; }
        [Display(Name ="Blood Group")]
        public string BloogGroup { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Height { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }
        public string Religion { get; set; }
        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Display(Name = "Photo Path")]
        public string? PhotoPath { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
