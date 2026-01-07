    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL;
using BLL.DTO;

namespace API_Adm.Controllers
{
    [ApiController]
    [Route("api/admin/farm")]
    [Authorize(Roles = "admin")]
    public class FarmController : ControllerBase
    {
        private readonly FarmBusiness _farmBusiness;

        public FarmController(FarmBusiness farmBusiness)
        {
            _farmBusiness = farmBusiness;
        }

        [HttpGet]
        public IActionResult GetFarms()
        {
            var farms = _farmBusiness.GetAll();
            return Ok(new { success = true, data = farms });
        }

        [HttpGet("{id:int}")]
        public IActionResult GetFarmsByID(int id)
        {
            var farm = _farmBusiness.GetById(id);
            if (farm == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy farm"
                });
            }

            return Ok(new { success = true, data = farm });
        }

        [HttpPost]
        public IActionResult CreateFarm([FromBody] CreateFarmDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newFarmId = _farmBusiness.Create(request);
            return Ok(new
            {
                success = true,
                message = "Tạo farm thành công",
                farmId = newFarmId
            });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateFarm(int id, [FromBody] UpdateFarmDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = _farmBusiness.Update(id, request);
            if (!ok)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy farm để cập nhật"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật farm thành công"
            });
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteFarm(int id)
        {
            var ok = _farmBusiness.Delete(id);
            if (!ok)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy farm để xóa"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa farm thành công"
            });
        }
    }
}
