using System.ComponentModel.DataAnnotations;

namespace BALibrary.Academic
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
