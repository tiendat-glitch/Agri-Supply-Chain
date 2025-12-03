using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using static BLL.ProductBusiness;

namespace QL_chuoi_cung_ung_nong_san.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductBusiness bll;

        public ProductController(ProductBusiness service)
        {
            bll = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(bll.GetAllProducts());
        }

        [HttpGet("category/{category}")]
        public IActionResult GetByCategory(string category)
        {
            return Ok(bll.GetByCategory(category));
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string keyword)
        {
            return Ok(bll.SearchByName(keyword));
        }

        [HttpGet("sorted")]
        public IActionResult Sort()
        {
            return Ok(bll.SortByCreatedDate());
        }
    }
}
