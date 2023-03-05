using System.ComponentModel.DataAnnotations;

namespace BALibrary.Academic
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Room No")]
        public string RoomNo { get; set; }
        public int Capacity { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
