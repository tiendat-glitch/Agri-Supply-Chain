namespace API_Retailer.DTOs.Signature
{
    public class RetailerDigitalSignatureDto
    {
        public string SignatureValue { get; set; } = null!;
        public string SignatureMethod { get; set; } = null!;
        public DateTime SignedAt { get; set; }
        public string? SignedBy { get; set; }
        public string? ReferenceDocument { get; set; }
    }
}
