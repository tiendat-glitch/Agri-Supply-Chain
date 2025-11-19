using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL_chuoi_cung_ung_nong_san.Models;

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
