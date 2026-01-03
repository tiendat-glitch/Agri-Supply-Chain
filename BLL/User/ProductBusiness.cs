using DAL.Helper;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL
{
    public class ProductBusiness
    {
        private readonly ProductRepository dal;

        public ProductBusiness(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection");
            var helper = new DatabaseHelper(connectionString);
            dal = new ProductRepository(helper);
        }

        public List<Product> GetAllProducts()
        {
            return dal.GetAll();
        }

        public List<Product> GetByCategory(string category)
        {
            return dal.GetAll()
                        .Where(p => !string.IsNullOrEmpty(p.Category) &&
                                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }

        public List<Product> SearchByName(string keyword)
        {
            return dal.GetAll()
                        .Where(p => !string.IsNullOrEmpty(p.Name) &&
                                    p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }

        public List<Product> SortByCreatedDate()
        {
            return dal.GetAll()
                        .OrderByDescending(p => p.CreatedAt)
                        .ToList();
        }
    }
}
