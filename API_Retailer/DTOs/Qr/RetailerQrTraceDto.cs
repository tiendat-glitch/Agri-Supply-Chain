using API_Retailer.DTOs.Batch;
using API_Retailer.DTOs.Signature;

namespace API_Retailer.DTOs.Qr
{
    public class RetailerQrTraceDto
    {
        public string BatchCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string FarmName { get; set; } = null!;
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = null!;
        public string QrToken { get; set; } = null!;
        public string? QrUrl { get; set; }
        public List<RetailerDigitalSignatureDto> Signatures { get; set; } = new();

        public List<RetailerInspectionDto> Inspections { get; set; }
            = new();
    }

}
