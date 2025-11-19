using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_chuoi_cung_ung_nong_san.Models;
using System.Linq;
using static QL_chuoi_cung_ung_nong_san.Code.ProductModels;

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
            try
            {
                var products = db.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.Category,
                    p.StorageInstructions,
                    p.TypicalShelfLifeDays,
                    p.CreatedAt
                }).ToList();
                return Ok(products);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("get-by-id/{id}")]
        [HttpGet]
        public IActionResult GetById(int id)
        {          
            return Ok(db.Products.SingleOrDefault(p => p.Id == id));
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create([FromBody] ProductCreateModel model)
        {
            try
            {
                var Product = new Product
                {
                    Name = model.Name,
                    Sku = model.Sku,
                    Category = model.Category,
                    StorageInstructions = model.StorageInstructions,
                    TypicalShelfLifeDays = model.TypicalShelfLifeDays,
                    CreatedAt = DateTime.Now
                };
                db.Products.Add(Product);
                db.SaveChanges();
                return Ok(new { message = "Tạo sản phẩm thành công" });
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("update")]
        [HttpPost]
        public IActionResult Update([FromBody] ProductUpdateModel model)
        {
            try
            {
                var existingProduct = db.Products.SingleOrDefault(p => p.Id == model.Id);
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại" });
                }
                existingProduct.Name = model.Name;
                existingProduct.Sku = model.Sku;
                existingProduct.Category = model.Category;
                existingProduct.StorageInstructions = model.StorageInstructions;
                existingProduct.TypicalShelfLifeDays = model.TypicalShelfLifeDays;
                db.SaveChanges();
                return Ok(new { message = "Cập nhật sản phẩm thành công" });
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var existingProduct = db.Products.SingleOrDefault(p => p.Id == id);
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại" });
                }
                db.Products.Remove(existingProduct);
                db.SaveChanges();
                return Ok(new { message = "Xóa sản phẩm thành công" });
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("search")]
        [HttpPost]

    }
}
