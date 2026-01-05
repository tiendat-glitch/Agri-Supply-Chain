namespace BLL.DTO
{
    public class InspectionDTO
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int? InspectorUserId { get; set; }
        public DateTime InspectionDate { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? ChemicalResidue { get; set; }
        public int? QualityScore { get; set; }
        public string? ReportFile { get; set; }
        public int? SignatureId { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateInspectionDto
    {
        public int BatchId { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? ChemicalResidue { get; set; }
        public int? QualityScore { get; set; }
        public string? ReportFile { get; set; }
        public int? SignatureId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateInspectionDto
    {
        public decimal? Humidity { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? ChemicalResidue { get; set; }
        public int? QualityScore { get; set; }
        public string? ReportFile { get; set; }
        public int? SignatureId { get; set; }
        public string? Notes { get; set; }
    }
}


