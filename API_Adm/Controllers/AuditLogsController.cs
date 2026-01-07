using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Adm.Controllers
{
    [ApiController]
    [Route("api/admin/audit-logs")]
    [Authorize(Roles = "admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly AuditLogBusiness _business;

        public AuditLogsController(AuditLogBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<List<AuditLogDTO>> Get(
            [FromQuery] int? userId,
            [FromQuery] string? tableName,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var filter = new AuditLogFilterDto
            {
                UserId = userId,
                TableName = tableName,
                Action = action,
                From = from,
                To = to
            };

            var logs = _business.GetLogs(filter);
            return Ok(logs);
        }

        [HttpGet("all")]
        public ActionResult<List<AuditLogDTO>> GetAll()
        {
            var filter = new AuditLogFilterDto(); 
            var logs = _business.GetLogs(filter);
            return Ok(logs);
        }
    }
}
