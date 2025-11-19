using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL_chuoi_cung_ung_nong_san.Models;

namespace QL_chuoi_cung_ung_nong_san.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private AgriContext db = new AgriContext();
        [Route("get-all")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.Products.ToList());
        }

        [Route("get-by-id/{id}")]
        [HttpGet]
        public IActionResult GetById(int id)
        {          
            return Ok(db.Products.SingleOrDefault(p => p.Id == id));
        }

        [Route("")]
    }
}
