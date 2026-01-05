using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/qr/trace")]
    public class TraceController : ControllerBase
    {
        private readonly TraceBusiness _traceBusiness;

        public TraceController(TraceBusiness traceBusiness)
        {
            _traceBusiness = traceBusiness;
        }

        [AllowAnonymous]
        [HttpGet("{token}")]
        public ActionResult<TraceResultDto> Trace(string token)
        {
            var result = _traceBusiness.TraceByToken(token);
            return Ok(result);
        }
    }
}

