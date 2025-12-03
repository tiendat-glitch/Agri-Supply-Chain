using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using static BLL.ProductBusiness;

namespace QL_chuoi_cung_ung_nong_san.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchesController : ControllerBase
    {
        private AgriContext db = new AgriContext();
        [Route("get-all")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.Batches.ToList());
        }
    }
}
