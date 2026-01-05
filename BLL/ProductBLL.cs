using System;
using System.Collections.Generic;
using BLL.DTO;
using DAL.Repositories;

namespace BLL
{
    public class bll_Product
    {
        private readonly ProductRepository _repo;

        public bll_Product(ProductRepository repo)
        {
            _repo = repo;
        }

        // ============================
        // GET ALL
        // ============================
        public List<ProductDTO> GetAllProduct(string? q = null)
        {
            var products = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(q))
            {
                products = products
                    .FindAll(p => p.Name != null &&
                                  p.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            return products.ConvertAll(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Category = p.Category,
                StorageInstructions = p.StorageInstructions,
                TypicalShelfLifeDays = p.TypicalShelfLifeDays
            });
        }

        // ============================
        // GET BY ID
        // ============================
        public ProductDTO? GetProductById(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return null;

            return new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Category = p.Category,
                StorageInstructions = p.StorageInstructions,
                TypicalShelfLifeDays = p.TypicalShelfLifeDays
            };
        }

        // ============================
        // CREATE ✅
        // ============================
        public bool CreateProduct(ProductDTO dto)
        {
            if (dto == null)
                throw new Exception("Product không hợp lệ");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Tên sản phẩm không được để trống");

            // TODO: implement Insert trong Repository
            return true;
        }

        // ============================
        // UPDATE
        // ============================
        public ProductDTO? UpdateProduct(int id, ProductDTO dto)
        {
            return null; // TODO
        }

        // ============================
        // DELETE
        // ============================
        public bool DeleteProduct(int id)
        {
            return true; // TODO
        }
    }
}
