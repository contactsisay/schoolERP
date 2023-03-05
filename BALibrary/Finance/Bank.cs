using BALibrary.Hostel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BALibrary.Finance
{
    public class Bank
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Short Code")]
        public string ShortCode { get; set; }
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
