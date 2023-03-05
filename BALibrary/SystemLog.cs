using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALibrary
{
    public class SystemLog
    {
        [Key]
        public int SystemLogId { get; set; }
        [Required]
        public int ActionType { get; set; } //login, create, update and delete
        public string? AffectedFields { get; set; } //comma separated
        public string? BeforeChange { get; set; }//comma separated
        public string? AfterChange { get; set; }//comma separated
        public DateTime? LastUpdatedAt { get; set;}
        public DateTime? LastUpdatedBy { get; set; }
        public bool Commit { get; set; }

    }
}
