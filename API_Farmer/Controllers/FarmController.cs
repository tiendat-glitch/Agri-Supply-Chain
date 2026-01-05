using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Farmer.Controllers
{
    [ApiController]
    [Route("api/farmer/farm")]
    [Authorize(Roles = "farmer")]
    public class FarmController : ControllerBase
    {
        private readonly FarmBusiness _farmBusiness;

        public FarmController(FarmBusiness farmBusiness)
        {
            _farmBusiness = farmBusiness;
        }

        [HttpGet]
        public ActionResult<List<FarmDTO>> GetMyFarms()
        {
            var farms = _farmBusiness.GetAll();
            return Ok(farms);
        }
    }
}
