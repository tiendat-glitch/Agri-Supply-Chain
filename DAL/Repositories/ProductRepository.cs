using DAL.Helper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
    public class ProductRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ProductRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Product> GetAll()
        {
            List<Product> list = new List<Product>();
            using SqlConnection conn = _dbHelper.GetConnection();
            string query = "SELECT id, name, sku, category, storage_instructions, typical_shelf_life_days, created_at FROM dbo.products";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Product
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString(),
                    Sku = reader["sku"]?.ToString(),
                    Category = reader["category"]?.ToString(),
                    StorageInstructions = reader["storage_instructions"]?.ToString(),
                    TypicalShelfLifeDays = reader["typical_shelf_life_days"] as int?,
                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                });
            }
            return list;
        }
    }
}
