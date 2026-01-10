using Model;
using Microsoft.Data.SqlClient;

namespace DAL.Mappers
{
    public static class BatchMapper
    {
        public static Batch Map(SqlDataReader reader)
        {
            return new Batch
            {
                Id = (int)reader["id"],
                BatchCode = reader["batch_code"].ToString()!,

                HarvestDate = reader["harvest_date"] == DBNull.Value
                    ? null
                    : DateOnly.FromDateTime((DateTime)reader["harvest_date"]),

                ExpiryDate = reader["expiry_date"] == DBNull.Value
                    ? null
                    : DateOnly.FromDateTime((DateTime)reader["expiry_date"]),

                Status = reader["status"].ToString()!,
                CreatedAt = (DateTime)reader["created_at"],

                CreatedByUserId = reader["created_by_user_id"] == DBNull.Value
                    ? null
                    : (int?)reader["created_by_user_id"],

                Quantity = reader["quantity"] == DBNull.Value
                    ? null
                    : (decimal?)reader["quantity"],

                Unit = reader["unit"] == DBNull.Value
                    ? null
                    : reader["unit"].ToString(),

                ProductId = (int)reader["product_id"],

                Product = new Product
                {
                    Id = (int)reader["product_id"],
                    Name = reader["product_name"].ToString()!
                }
            };
        }
        // dùng cho GetByProductId
        public static Batch MapSimple(SqlDataReader r)
        {
            return new Batch
            {
                Id = (int)r["id"],
                BatchCode = r["batch_code"].ToString()!,
                ProductId = (int)r["product_id"],

                Quantity = r["quantity"] == DBNull.Value
                    ? null
                    : (decimal?)r["quantity"],

                Unit = r["unit"] == DBNull.Value
                    ? null
                    : r["unit"].ToString(),

                Status = r["status"].ToString()!,
                CreatedAt = (DateTime)r["created_at"]
            };
        }

        // dùng cho GetByFarmId
        public static Batch MapForFarm(SqlDataReader r)
        {
            return new Batch
            {
                Id = (int)r["id"],
                BatchCode = r["batch_code"].ToString()!,
                ProductId = (int)r["product_id"],
                FarmId = (int)r["farm_id"],

                HarvestDate = r["harvest_date"] == DBNull.Value
                    ? null
                    : DateOnly.FromDateTime((DateTime)r["harvest_date"]),

                Quantity = r["quantity"] == DBNull.Value
                    ? null
                    : (decimal?)r["quantity"],

                Unit = r["unit"] == DBNull.Value
                    ? null
                    : r["unit"].ToString(),

                Status = r["status"].ToString()!,
                CreatedAt = (DateTime)r["created_at"]
            };
        }
    }
}
