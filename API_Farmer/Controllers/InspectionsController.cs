using System.Collections.Generic;
using System.Security.Claims;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Inspection.Controllers
{
    [ApiController]
    [Route("api/farmer/inspection")]
    [Authorize(Roles = "farmer")]
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
    }
}


