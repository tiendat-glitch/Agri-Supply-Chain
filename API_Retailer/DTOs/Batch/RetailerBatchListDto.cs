namespace API_Retailer.DTOs.Batch
{
    public class RetailerBatchListDto
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; } = null!;
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = null!;
    }

}
