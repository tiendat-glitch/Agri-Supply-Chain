namespace API_Retailer.DTOs.Qr
{
    public class RetailerQrTraceAnalyzeDto
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; } = null!;

        public int InspectionCount { get; set; }
        public double AverageQualityScore { get; set; }

        public decimal MaxChemicalResidue { get; set; }
        public decimal MaxTemperature { get; set; }
        public decimal MaxHumidity { get; set; }

        public string RiskLevel { get; set; } = null!;
        public string Decision { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }
}
