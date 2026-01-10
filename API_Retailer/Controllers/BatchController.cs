using API_Retailer.DTOs.Batch;
using API_Retailer.DTOs.Qr;
using API_Retailer.DTOs.Signature;
using BLL;
using BLL.Retailer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Retailer.Controllers
{
    [ApiController]
    [Route("api/retailer/batches")]
    [Authorize(Roles = "retailer")]
    public class BatchController : ControllerBase
    {
        private readonly RetailerBatchBusiness _business;

        public BatchController(RetailerBatchBusiness business)
        {
            _business = business;
        }

        // GET api/retailer/batches
        [HttpGet]
        public IActionResult GetAll()
        {
            var batches = _business.GetAllBatches();
            var dtoList = batches.Select(b => new RetailerBatchListDto
            {
                BatchId = b.Id,
                BatchCode = b.BatchCode,
                HarvestDate = b.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = b.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = b.Status
            }).ToList();

            return Ok(dtoList);
        }

        // GET api/retailer/batches/9
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var batch = _business.GetBatchById(id);
            if (batch == null) return NotFound();

            var dto = new RetailerBatchDetailDto
            {
                BatchId = batch.Id,
                BatchCode = batch.BatchCode,
                ProductName = batch.Product.Name,
                HarvestDate = batch.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = batch.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = batch.Status,

                Source = null!
            };

            return Ok(dto);
        }

        // GET api/retailer/batches/by-product/3
        [HttpGet("by-product/{productId:int}")]
        public IActionResult GetByProduct(int productId)
        {
            var batches = _business.GetByProductId(productId);
            var dtoList = batches.Select(b => new RetailerBatchListDto
            {
                BatchId = b.Id,
                BatchCode = b.BatchCode,
                HarvestDate = b.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = b.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = b.Status
            }).ToList();

            return Ok(dtoList);
        }

        
        [HttpGet("qr-trace")]
        public IActionResult GetQrTrace()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            int retailerId = _business.GetRetailerIdByUserId(userId);

            var batches = _business.GetQrTrace(retailerId);

            var result = batches.Select(b => new RetailerQrTraceDto
            {
                BatchCode = b.BatchCode,
                ProductName = b.Product.Name,
                FarmName = b.Farm.Name,
                HarvestDate = b.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = b.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = b.Status,
                QrToken = b.QrCode?.Token ?? "",
                QrUrl = b.QrCode?.Url,

                Inspections = b.Inspections.Select(i => new RetailerInspectionDto
                {
                    InspectionDate = i.InspectionDate,
                    QualityScore = i.QualityScore,
                    Temperature = i.Temperature,
                    Humidity = i.Humidity,
                    ChemicalResidue = i.ChemicalResidue
                }).ToList(),

                Signatures = b.Inspections
                    .Where(i => i.Signature != null)
                    .Select(i => new RetailerDigitalSignatureDto
                    {
                        SignatureValue = i.Signature!.SignatureValue,
                        SignatureMethod = i.Signature!.SignatureMethod,
                        SignedAt = i.Signature!.SignedAt,
                        ReferenceDocument = i.Signature!.ReferenceDocument,
                        SignedBy = i.Signature!.SignerUser?.FullName
                    }).ToList()
            }).ToList();

            return Ok(result);
        }
        [HttpGet("qr-trace/analyze")]
        public IActionResult AnalyzeQrTrace()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            int retailerId = _business.GetRetailerIdByUserId(userId);

            var results = _business.AnalyzeQrTrace(retailerId);
            var response = results.Select(r => new RetailerQrTraceAnalyzeDto
            {
                BatchId = r.BatchId,
                BatchCode = r.BatchCode,
                InspectionCount = r.InspectionCount,
                AverageQualityScore = r.AverageQualityScore,
                MaxChemicalResidue = r.MaxChemicalResidue ?? 0m,
                MaxTemperature = r.MaxTemperature ?? 0m,
                MaxHumidity = r.MaxHumidity ?? 0m,
                RiskLevel = r.RiskLevel,
                Decision = r.Decision,
                Reason = r.Reason
            });

            return Ok(response);
        }
    }
}
