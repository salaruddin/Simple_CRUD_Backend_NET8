using backend.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos
{
    public class ProductCreateDTO
    {
        
        [Required]
        [MaxLength(20)]
        public required string Name { get; set; }

        [Required]
        public int CategoryID { get; set; }

       
    }
}
