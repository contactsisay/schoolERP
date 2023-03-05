using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Hostel
{
    public class HostelRoom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoomNo { get; set; }
        [Required]
        [Display(Name = "Hostel")]
        public int HostelId { get; set; }
        [Required]
        [Display(Name = "Number of Bed")]
        public int NumberOfBed { get; set; }
        [Required]
        [Display(Name = "Cost Per Bed")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPerBed { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ForeignKey("HostelId")]
        public Hostel? Hostel { get; set; }
    }
}
