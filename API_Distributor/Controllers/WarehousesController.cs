using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Warehouse.Controllers
{
    [ApiController]
    [Route("api/distributor/warehouse")]
    [Authorize(Roles = "distributor")]
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

    }
}


