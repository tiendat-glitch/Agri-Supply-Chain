namespace BLL.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Sku { get; set; }
        public string? Category { get; set; }
        public string? StorageInstructions { get; set; }
        public int? TypicalShelfLifeDays { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Sku { get; set; }
        public string? Category { get; set; }
        public string? StorageInstructions { get; set; }
        public int? TypicalShelfLifeDays { get; set; }
    }

    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Sku { get; set; }
        public string? Category { get; set; }
        public string? StorageInstructions { get; set; }
        public int? TypicalShelfLifeDays { get; set; }
    }
}


