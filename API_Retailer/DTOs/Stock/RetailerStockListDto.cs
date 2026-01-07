namespace API_Retailer.DTOs.Stock
{
    public class RetailerStockListDto
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public string BatchStatus { get; set; } = null!;
    }

}
