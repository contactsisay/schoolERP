using System.ComponentModel.DataAnnotations;

namespace BALibrary.Academic
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Is Practical?")]
        public bool IsPractical { get; set; } = false;
        public string Code { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
