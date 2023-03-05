using System.ComponentModel.DataAnnotations;

namespace BALibrary.Inventory
{
    public class Uom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
