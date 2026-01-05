using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/qr")]
    public class QrController : ControllerBase
    {
        private readonly QrBusiness _qrBusiness;

        public QrController(QrBusiness qrBusiness)
        {
            _qrBusiness = qrBusiness;
        }

        // Sinh/regen QR cho batch (admin hoặc farmer)
        [Authorize(Roles = "admin,farmer")]
        [HttpPost("{batchId:int}")]
        public ActionResult<QrCodeDTO> Generate(int batchId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var qr = _qrBusiness.GenerateForBatch(batchId, baseUrl);
            return Ok(qr);
        }
    }
}

