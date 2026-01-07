using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Farmer.Controllers
{
    [ApiController]
    [Route("api/farmer/batch")]
    [Authorize(Roles = "farmer")]
    public class BatchesController : ControllerBase
    {
        private readonly BatchBusiness _batchBusiness;

        public BatchesController(BatchBusiness batchBusiness)
        {
            _batchBusiness = batchBusiness;
        }

        // Lấy tất cả batch
        [HttpGet]
        public ActionResult<List<BatchDTO>> GetAll()
        {
            var list = _batchBusiness.GetAll();
            return Ok(list);
        }

        // Lấy theo id
        [HttpGet("{id:int}")]
        public ActionResult<BatchDTO> GetById(int id)
        {
            var batch = _batchBusiness.GetById(id);
            if (batch == null) return NotFound();
            return Ok(batch);
        }

        // Tạo batch
        [HttpPost]
        public IActionResult Create([FromBody] CreateBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            int? currentUserId = GetCurrentUserId();

            var id = _batchBusiness.Create(dto, currentUserId);
            return Ok(new { success = true, message = "Tạo batch thành công", batchId = id });
        }

        // Cập nhật batch
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? currentUserId = GetCurrentUserId();

            var ok = _batchBusiness.Update(id, dto, currentUserId);
            if (!ok) return NotFound("Không tìm thấy batch để cập nhật");

            return Ok(new { success = true, message = "Cập nhật batch thành công" });
        }

        // Xóa batch
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            int? currentUserId = GetCurrentUserId();

            var ok = _batchBusiness.Delete(id, currentUserId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Cập nhật trạng thái batch
        [HttpPut("{id:int}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] string status)
        {
            int? currentUserId = GetCurrentUserId();

            var ok = _batchBusiness.UpdateStatus(id, status, currentUserId);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Cập nhật trạng thái batch thành công" });
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var uid))
                return uid;
            return null;
        }
    }
}
