namespace API_Adm.DTO
{
    public class CreateBatchRequest
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

    public class UpdateBatchRequest
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
