namespace API_Adm.Models
{
    public class Batch
    {
        public int Id { get; set; }

        public string BatchCode { get; set; } = null!;

        public int ProductId { get; set; }

        public int FarmId { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateOnly? HarvestDate { get; set; }

        public decimal? Quantity { get; set; }

        public string? Unit { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
