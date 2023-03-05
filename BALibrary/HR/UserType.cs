using System.ComponentModel.DataAnnotations;

namespace BALibrary.HR
{
    public class UserType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
