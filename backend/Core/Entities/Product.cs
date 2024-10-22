using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Core.Entities
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Name { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; }= DateTime.UtcNow;

        [ForeignKey(nameof(Category))]
        public int CategoryID { get; set; }
        
        [Required]
        public  Category Category { get; set; }

    }
}
