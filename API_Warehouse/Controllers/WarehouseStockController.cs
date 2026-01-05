using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Warehouse.Controllers
{
    [ApiController]
    [Route("api/warehouse/{warehouseId:int}/stock")]
    [Authorize(Roles = "admin")]
    public class WarehouseStockController : ControllerBase
    {
        private readonly WarehouseBusiness _warehouseBusiness;

        public WarehouseStockController(WarehouseBusiness warehouseBusiness)
        {
            _warehouseBusiness = warehouseBusiness;
        }

        [HttpGet]
        public ActionResult<List<WarehouseStockDTO>> GetStocks(int warehouseId)
        {
            var list = _warehouseBusiness.GetStocks(warehouseId);
            return Ok(list);
        }

        [HttpPost("adjust")]
        public ActionResult<WarehouseStockDTO> Adjust(int warehouseId, [FromBody] AdjustWarehouseStockDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            dto.WarehouseId = warehouseId;
            var result = _warehouseBusiness.AdjustStock(dto);
            return Ok(result);
        }
    }
}


