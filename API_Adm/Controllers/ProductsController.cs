using System.Collections.Generic;
using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Product.Controllers
{
    [ApiController]
    [Route("api/admin/product")]
    [Authorize(Roles = "admin")]
    public class ProductsController : ControllerBase
    {
        private readonly bll_Product _productBll;

        public ProductsController(bll_Product productBll)
        {
            _productBll = productBll;
        }

        [HttpGet]
        public ActionResult<List<ProductDTO>> GetAll([FromQuery] string? q = null)
        {
            var list = _productBll.GetAllProduct(q);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ProductDTO> GetById(int id)
        {
            var product = _productBll.GetProductById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            var ok = _productBll.CreateProduct(dto);
            if (!ok)
                return BadRequest();

            return Ok(new { success = true, message = "Thêm product thành công" });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            var updatedProduct = _productBll.UpdateProduct(id, dto);
            if (updatedProduct == null)
                return NotFound("Không tìm thấy sản phẩm cần cập nhật");

            return Ok(new { success = true, message = "Cập nhật thành công", data = updatedProduct });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _productBll.DeleteProduct(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}


