using System.ComponentModel.DataAnnotations;

namespace BALibrary.Admin
{
    public class ModuleTable
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Select Module")]
        [Display(Name = "Module Name")]
        public int ModuleId { get; set; } //from constant (enum)

        [Required(ErrorMessage = "Please Select Table")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; } //from constant (enum)

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
