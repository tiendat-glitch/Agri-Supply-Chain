namespace BLL.DTO
{
    public class QrCodeDTO
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string Token { get; set; } = null!;
        public string? Url { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class TraceResultDto
    {
        public BatchDTO Batch { get; set; } = null!;
        public FarmDTO Farm { get; set; } = null!;
        public ProductDTO Product { get; set; } = null!;
        public List<InspectionDTO> Inspections { get; set; } = new();
        public List<ShipmentDTO> Shipments { get; set; } = new();
    }
}

