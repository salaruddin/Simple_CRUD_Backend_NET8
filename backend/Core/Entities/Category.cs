using System.ComponentModel.DataAnnotations;

namespace backend.Core.Entities
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(20)]
        public required string Name { get; set; }

        public IList<Product> Products { get; set; } = new List<Product>();
    }
}
