namespace API_Retailer.DTOs.Batch
{
    public class RetailerBatchDetailDto
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = null!;

        public RetailerBatchSourceDto Source { get; set; } = null!;
    }

}
