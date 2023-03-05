using System.ComponentModel.DataAnnotations;

namespace BALibrary.HR
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
