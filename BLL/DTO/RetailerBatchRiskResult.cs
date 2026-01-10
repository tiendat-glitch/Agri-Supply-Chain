using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class RetailerBatchRiskResult
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; } = null!;

        public int InspectionCount { get; set; }
        public double AverageQualityScore { get; set; }

        public decimal? MaxChemicalResidue { get; set; }
        public decimal? MaxTemperature { get; set; }
        public decimal? MaxHumidity { get; set; }

        public string RiskLevel { get; set; } = null!; // LOW | MEDIUM | HIGH
        public string Decision { get; set; } = null!;  // SELL | HOLD | RECALL
        public string Reason { get; set; } = null!;
    }
}
