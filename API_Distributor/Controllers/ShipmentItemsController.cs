using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Shipment.Controllers
{
    [ApiController]
    [Route("api/distributor/shipment/{shipmentId:int}/items")]
    [Authorize(Roles = "distributor")]
    public class ShipmentItemsController : ControllerBase
    {
        private readonly ShipmentBusiness _business;

        public ShipmentItemsController(ShipmentBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public ActionResult<List<ShipmentItemDTO>> GetItems(int shipmentId)
        {
            var items = _business.GetItems(shipmentId);
            return Ok(items);
        }

        [HttpPost]
        public IActionResult AddItem(int shipmentId, [FromBody] CreateShipmentItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var id = _business.AddItem(shipmentId, dto);
            return Ok(new { success = true, message = "Thêm item thành công", itemId = id });
        }

        [HttpPut("{itemId:int}")]
        public IActionResult UpdateItem(int shipmentId, int itemId, [FromBody] CreateShipmentItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var ok = _business.UpdateItem(itemId, dto);
            if (!ok) return NotFound();
            return Ok(new { success = true, message = "Cập nhật item thành công" });
        }

        [HttpDelete("{itemId:int}")]
        public IActionResult DeleteItem(int shipmentId, int itemId)
        {
            var ok = _business.DeleteItem(itemId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

