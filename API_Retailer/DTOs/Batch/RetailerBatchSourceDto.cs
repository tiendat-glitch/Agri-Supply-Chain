namespace API_Retailer.DTOs.Batch
{
    public class RetailerBatchSourceDto
    {
        public string FarmName { get; set; } = null!;
        public string? FarmLocation { get; set; }

        public List<RetailerInspectionDto> Inspections { get; set; }
            = new();
    }
}
