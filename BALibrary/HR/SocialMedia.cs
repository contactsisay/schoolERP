using System.ComponentModel.DataAnnotations;

namespace BALibrary.HR
{
    public class SocialMedia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Icon Path")]
        public string IconPath { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
