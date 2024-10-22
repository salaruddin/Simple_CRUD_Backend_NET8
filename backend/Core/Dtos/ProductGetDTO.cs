using backend.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Core.Dtos
{
    public class ProductGetDTO
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; } 
        public int CategoryID { get; set; }

    }
}
