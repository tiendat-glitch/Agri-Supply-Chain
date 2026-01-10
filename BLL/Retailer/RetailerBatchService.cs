using DAL.Helper;
using DAL.Repositories;
using BLL.DTO;
using Microsoft.Extensions.Configuration;
using Model;

namespace BLL.Retailer
{
    public class RetailerBatchBusiness
    {
        private readonly BatchRepository _batchRepo;

        public RetailerBatchBusiness(IConfiguration config)
        {
            string connStr = config.GetConnectionString("DefaultConnection");
            var helper = new DatabaseHelper(connStr);
            _batchRepo = new BatchRepository(helper);
        }

        // Lấy tất cả batch mà retailer được xem
        public List<Batch> GetAllBatches()
        {
            return _batchRepo.GetAll();
        }
        public int GetRetailerIdByUserId(int userId)
        {
            return _batchRepo.GetRetailerIdByUserId(userId);
        }
        //Truy xuất nguồn gốc
        public List<Batch> GetQrTrace(int retailerId)
        {
            return _batchRepo.GetQrTraceForRetailer(retailerId);
        }
        public List<RetailerBatchRiskResult> AnalyzeQrTrace(int retailerId)
        {
            var batches = _batchRepo.GetQrTraceForRetailer(retailerId);
            var results = new List<RetailerBatchRiskResult>();

            foreach (var batch in batches)
            {
                if (batch.Inspections == null || !batch.Inspections.Any())
                    continue;

                var inspections = batch.Inspections
                    .GroupBy(i => i.InspectionDate)
                    .Select(g => g.First())
                    .ToList();

                var validScores = inspections
                    .Where(i => i.QualityScore.HasValue)
                    .Select(i => (double)i.QualityScore!.Value)
                    .ToList();

                if (!validScores.Any())
                    continue;

                var avgScore = validScores.Average();
                var maxChemical = inspections.Max(i => i.ChemicalResidue);
                var maxTemp = inspections.Max(i => i.Temperature);
                var maxHumidity = inspections.Max(i => i.Humidity);

                string risk;
                string decision;
                string reason;

                if (avgScore < 70 || maxChemical > 0.01m)
                {
                    risk = "HIGH";
                    decision = "RECALL";
                    reason = "Chất lượng thấp hoặc dư lượng hóa chất vượt ngưỡng cho phép";
                }
                else if (avgScore < 85)
                {
                    risk = "MEDIUM";
                    decision = "HOLD";
                    reason = "Chất lượng trung bình, cần theo dõi thêm";
                }
                else
                {
                    risk = "LOW";
                    decision = "SELL";
                    reason = "Lô hàng đạt tiêu chuẩn an toàn";
                }

                results.Add(new RetailerBatchRiskResult
                {
                    BatchId = batch.Id,
                    BatchCode = batch.BatchCode,
                    InspectionCount = inspections.Count,
                    AverageQualityScore = Math.Round(avgScore, 2),
                    MaxChemicalResidue = maxChemical,
                    MaxTemperature = maxTemp,
                    MaxHumidity = maxHumidity,
                    RiskLevel = risk,
                    Decision = decision,
                    Reason = reason
                });
            }

            return results;
        }

        // Chi tiết batch
        public Batch GetBatchById(int id)
        {
            return _batchRepo.GetById(id)
                   ?? throw new Exception("Batch không tồn tại");
        }

        // Theo product
        public List<Batch> GetByProductId(int productId)
        {
            return _batchRepo.GetByProductId(productId);
        }

        // Theo farm
        public List<Batch> GetByFarmId(int farmId)
        {
            return _batchRepo.GetByFarmId(farmId);
        }
    }
}
