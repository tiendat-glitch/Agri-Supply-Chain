using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Warehouse.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    [Authorize(Roles = "admin")]
    public class WarehousesController : ControllerBase
    {
        private readonly WarehouseBusiness _warehouseBusiness;

        public WarehousesController(WarehouseBusiness warehouseBusiness)
        {
            _warehouseBusiness = warehouseBusiness;
        }

        [HttpGet]
        public ActionResult<List<WarehouseDTO>> GetAll()
        {
            var list = _warehouseBusiness.GetAllWarehouses();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<WarehouseDTO> GetById(int id)
        {
            var warehouse = _warehouseBusiness.GetWarehouse(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateWarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var id = _warehouseBusiness.CreateWarehouse(dto);
            return Ok(new { success = true, message = "Tạo kho thành công", warehouseId = id });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var ok = _warehouseBusiness.UpdateWarehouse(id, dto);
            if (!ok) return NotFound();

            return Ok(new { success = true, message = "Cập nhật kho thành công" });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _warehouseBusiness.DeleteWarehouse(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


