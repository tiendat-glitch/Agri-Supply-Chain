namespace API_Retailer.DTOs.Batch
{
    public class RetailerInspectionDto
    {
        public DateTime InspectionDate { get; set; }
        public int? QualityScore { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? ChemicalResidue { get; set; }
    }
}
