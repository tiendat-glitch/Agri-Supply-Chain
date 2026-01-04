using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Adm.Models
{
    [Table("products")]
    public class Product
    {
        public Product()
        {
            Batches = new HashSet<Batch>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? Sku { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? StorageInstructions { get; set; }

        public int? TypicalShelfLifeDays { get; set; }

        // Navigation
        public virtual ICollection<Batch> Batches { get; set; }
    }
}