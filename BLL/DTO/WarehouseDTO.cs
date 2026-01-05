namespace BLL.DTO
{
    public class WarehouseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateWarehouseDto
    {
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateWarehouseDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public bool? IsActive { get; set; }
    }

    public class WarehouseStockDTO
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int BatchId { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class AdjustWarehouseStockDto
    {
        public int WarehouseId { get; set; }
        public int BatchId { get; set; }
        public decimal QuantityDelta { get; set; }
        public string? Unit { get; set; }
    }
}


