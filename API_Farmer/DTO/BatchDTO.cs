namespace BatchDTOs.DTO
{
    public class BatchDTO
    {
        public int Id { get; set; }
        public string BatchCode { get; set; } = null!;
        public int ProductId { get; set; }
        public int FarmId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? HarvestDate { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBatchDto
    {
        public string BatchCode { get; set; } = null!;
        public int ProductId { get; set; }
        public int FarmId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? HarvestDate { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateBatchDto
    {
        public string? BatchCode { get; set; }
        public int? ProductId { get; set; }
        public int? FarmId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? HarvestDate { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
    }
}


