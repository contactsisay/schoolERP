using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Hostel
{
    public class Hostel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        [Required]
        [Display(Name = "Intake")]
        public string Intake { get; set; }
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ForeignKey("RoomTypeId")]
        public RoomType? RoomType { get; set; }
    }
}
