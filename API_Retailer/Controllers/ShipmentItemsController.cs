using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Shipment.Controllers
{
    [ApiController]
    [Route("api/retailer/shipment/{shipmentId:int}/items")]
    [Authorize(Roles = "retailer")]
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
    }
}

