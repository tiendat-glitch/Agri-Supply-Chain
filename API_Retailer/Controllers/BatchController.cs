using API_Retailer.DTOs.Batch;
using API_Retailer.DTOs.Qr;
using API_Retailer.DTOs.Signature;
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

        // GET api/retailer/batches/by-farm/2
        [HttpGet("by-farm/{farmId:int}")]
        public IActionResult GetByFarm(int farmId)
        {
            var batches = _business.GetByFarmId(farmId);
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
            // Lấy UserId từ JWT token
            int retailerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
    }
}
