using System.ComponentModel.DataAnnotations;

namespace BALibrary.Sales
{
    public class CustomerType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
