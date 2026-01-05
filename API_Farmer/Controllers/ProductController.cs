using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL;
using BLL.DTO;

namespace API_Farmer.Controllers
{
    [ApiController]
    [Route("api/farmer/product")]
    [Authorize(Roles = "farmer")]
    public class ProductsController : ControllerBase
    {
        private readonly bll_Product _productBll;

        public ProductsController(bll_Product productBll)
        {
            _productBll = productBll;
        }

        // GET: api/products?q=search
        [HttpGet]
        public ActionResult<List<ProductDTO>> GetAll([FromQuery] string? q = null)
        {
            var list = _productBll.GetAllProduct(q);
            return Ok(list);
        }

        // GET: api/products/5
        [HttpGet("{id:int}")]
        public ActionResult<ProductDTO> GetById(int id)
        {
            var product = _productBll.GetProductById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        
    }
}
