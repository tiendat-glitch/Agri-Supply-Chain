using System.Collections.Generic;
using System.Security.Claims;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Shipment.Controllers
{
    [ApiController]
    [Route("api/farmer/shipment")]
    [Authorize(Roles = "farmer")]
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
    }
}

