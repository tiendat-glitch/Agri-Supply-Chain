using System.Collections.Generic;
using System.Security.Claims;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Inspection.Controllers
{
    [ApiController]
    [Route("api/admin/inspection")]
    [Authorize(Roles = "admin")]
    public class InspectionsController : ControllerBase
    {
        private readonly InspectionBusiness _inspectionBusiness;

        public InspectionsController(InspectionBusiness inspectionBusiness)
        {
            _inspectionBusiness = inspectionBusiness;
        }

        [HttpGet]
        public ActionResult<List<InspectionDTO>> GetAll()
        {
            var list = _inspectionBusiness.GetAll();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<InspectionDTO> GetById(int id)
        {
            var item = _inspectionBusiness.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateInspectionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            int? inspectorId = null;
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out var uid))
                inspectorId = uid;

            var id = _inspectionBusiness.Create(dto, inspectorId);
            return Ok(new { success = true, message = "Tạo inspection thành công", inspectionId = id });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateInspectionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            var ok = _inspectionBusiness.Update(id, dto);
            if (!ok) return NotFound();

            return Ok(new { success = true, message = "Cập nhật inspection thành công" });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _inspectionBusiness.Delete(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


