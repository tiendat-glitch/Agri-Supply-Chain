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

        /// <summary>
        /// L?y audit log theo filter (có th? không truy?n gì ?? l?y t?t c?)
        /// GET /api/admin/audit-logs?userId=1&tableName=batches&action=CREATE
        /// </summary>
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

            var logs = _business.GetLogs(filter); // N?u t?t c? null ? l?y t?t c?
            return Ok(logs);
        }

        /// <summary>
        /// L?y t?t c? audit log (không c?n truy?n gì)
        /// GET /api/admin/audit-logs/all
        /// </summary>
        [HttpGet("all")]
        public ActionResult<List<AuditLogDTO>> GetAll()
        {
            var filter = new AuditLogFilterDto(); // r?ng ? l?y toàn b?
            var logs = _business.GetLogs(filter);
            return Ok(logs);
        }
    }
}
