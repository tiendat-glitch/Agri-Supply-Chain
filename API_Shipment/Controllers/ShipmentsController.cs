using System.Collections.Generic;
using System.Security.Claims;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Shipment.Controllers
{
    [ApiController]
    [Route("api/shipment")]
    [Authorize(Roles = "admin")]
    public class ShipmentsController : ControllerBase
    {
        private readonly ShipmentBusiness _business;

        public ShipmentsController(ShipmentBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<List<ShipmentDTO>> GetAll()
        {
            var list = _business.GetAll();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ShipmentDTO> GetById(int id)
        {
            var item = _business.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateShipmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            int plannerId = 0;
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(idClaim, out plannerId);

            var id = _business.CreateShipment(dto, plannerId);
            return Ok(new { success = true, message = "Tạo shipment thành công", shipmentId = id });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateShipmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var ok = _business.UpdateShipment(id, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Cập nhật shipment thành công" });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _business.DeleteShipment(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] string status)
        {
            var ok = _business.UpdateStatus(id, status);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Cập nhật trạng thái shipment thành công" });
        }
    }
}

