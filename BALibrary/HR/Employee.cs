using BALibrary.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.HR
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "User Type")]
        public int UserTypeId { get; set; }
        [Required]
        [Display(Name = "Job Position")]
        public int JobPositionId { get; set; }
        [Required]
        [Display(Name = "Employment Type")]
        public int EmploymentTypeId { get; set; }
        public string? Designation { get; set; }
        public string Code { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Mother Name")]
        public string? MotherName { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        [Display(Name = "Joined Date")]
        public DateTime JoinedDate { get; set; }
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [Display(Name = "Emergency Contact No")]
        public string? EmergencyContactNo { get; set; }
        [Display(Name = "Marital Status")]
        public string? MaritalStatus { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Current Address")]
        public string? CurrentAddress { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Permanent Address")]
        public string? PermanentAddress { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
        [Display(Name = "Photo Path")]
        public string? PhotoPath { get; set; }
        public string? Qualification { get; set; }
        [Display(Name = "Work Experience")]
        public int WorkExperience { get; set; } = 0;
        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("EmploymentTypeId")]
        public virtual EmploymentType? EmploymentType { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserType? UserType { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
        [ForeignKey("JobPositionId")]
        public virtual JobPosition? JobPosition { get; set; }
    }
}
