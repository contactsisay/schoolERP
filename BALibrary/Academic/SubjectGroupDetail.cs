using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Academic
{
    public class SubjectGroupDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Subject Group")]
        public int SubjectGroupId { get; set; }
        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }

        [ForeignKey("SubjectGroupId")]
        public virtual SubjectGroup? SubjectGroup { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }
    }
}
