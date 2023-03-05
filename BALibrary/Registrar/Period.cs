using System.ComponentModel.DataAnnotations;

namespace BALibrary.Registrar
{
    public class Period
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } //1, 2, etc or 
        [Required]
        [Display(Name ="From")]
        public string FromTime { get; set; }
        [Required]
        [Display(Name ="To")]
        public string ToTime { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}