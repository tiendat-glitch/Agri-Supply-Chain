using DAL.Helper;
using DAL.Mappers;
using Microsoft.Data.SqlClient;
using Model;
using System.Data;

namespace DAL.Repositories
{
    public class BatchRepository
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly AuditHelper _auditHelper;

        public BatchRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _auditHelper = new AuditHelper(dbHelper);
        }

        public List<Batch> GetAll()
        {
            var list = new List<Batch>();
            using var conn = _dbHelper.GetConnection();

            const string sql = @"
                SELECT 
                    b.id, b.batch_code, b.harvest_date, b.expiry_date,
                    b.status, b.created_at,
                    p.id AS product_id, p.name AS product_name
                FROM dbo.batches b
                INNER JOIN dbo.products p ON b.product_id = p.id
                ORDER BY b.created_at DESC";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Batch
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

                    Product = new Product
                    {
                        Id = (int)reader["product_id"],
                        Name = reader["product_name"].ToString()!
                    }
                });
            }

            return list;
        }
        public Batch? GetById(int id)
        {
            using var conn = _dbHelper.GetConnection();

            using var cmd = new SqlCommand("SP_Batch_GetById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return BatchMapper.Map(reader);
        }

        public List<Batch> GetQrTraceForRetailer(int retailerId)
        {
            var batches = new Dictionary<int, Batch>();

            using var conn = _dbHelper.GetConnection();
            using var cmd = new SqlCommand("SP_Retailer_GetQrTrace_v3", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@RetailerId", retailerId);

            using var reader = cmd.ExecuteReader();

            //Batch + Product + Farm + QR
            while (reader.Read())
            {
                int batchId = (int)reader["batch_id"];

                if (!batches.ContainsKey(batchId))
                {
                    batches[batchId] = new Batch
                    {
                        Id = batchId,
                        BatchCode = reader["batch_code"]?.ToString() ?? "",
                        Status = reader["status"]?.ToString() ?? "",
                        HarvestDate = reader["harvest_date"] is DateTime hd
                            ? DateOnly.FromDateTime(hd)
                            : null,
                        ExpiryDate = reader["expiry_date"] is DateTime ed
                            ? DateOnly.FromDateTime(ed)
                            : null,

                        Product = new Product
                        {
                            Id = (int)reader["product_id"],
                            Name = reader["product_name"]?.ToString() ?? ""
                        },

                        Farm = new Farm
                        {
                            Id = (int)reader["farm_id"],
                            Name = reader["farm_name"]?.ToString() ?? "",
                            Location = reader["farm_location"]?.ToString()
                        },

                        QrCode = reader["qr_id"] != DBNull.Value
                            ? new QrCode
                            {
                                Id = (int)reader["qr_id"],
                                Token = reader["qr_token"]?.ToString() ?? "",
                                Url = reader["qr_url"]?.ToString()
                            }
                            : null,

                        Inspections = new List<Inspection>(),
                        RetailerStocks = new List<RetailerStock>()
                    };
                }
            }

            //Inspections + Digital Signature
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    int batchId = (int)reader["batch_id"];

                    if (!batches.TryGetValue(batchId, out var batch))
                        continue;

                    var inspection = new Inspection
                    {
                        Id = (int)reader["inspection_id"],
                        InspectionDate = (DateTime)reader["inspection_date"],
                        QualityScore = reader["quality_score"] as int?,
                        Temperature = reader["temperature"] as decimal?,
                        Humidity = reader["humidity"] as decimal?,
                        ChemicalResidue = reader["chemical_residue"] as decimal?,

                        Signature = reader["signature_id"] != DBNull.Value
                            ? new DigitalSignature
                            {
                                Id = (int)reader["signature_id"],
                                SignatureValue = reader["signature_value"]?.ToString() ?? "",
                                SignatureMethod = reader["signature_method"]?.ToString() ?? "",
                                SignedAt = (DateTime)reader["signed_at"],
                                ReferenceDocument = reader["reference_document"]?.ToString(),

                                SignerUser = reader["signer_id"] != DBNull.Value
                                    ? new User
                                    {
                                        Id = (int)reader["signer_id"],
                                        FullName = reader["signer_name"]?.ToString()
                                    }
                                    : null
                            }
                            : null
                    };

                    batch.Inspections.Add(inspection);
                }
            }

            //Retailer Stock
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    int batchId = (int)reader["batch_id"];

                    if (!batches.TryGetValue(batchId, out var batch))
                        continue;

                    batch.RetailerStocks.Add(new RetailerStock
                    {
                        Id = (int)reader["retailer_stock_id"],
                        Quantity = (decimal)reader["quantity"],
                        Unit = reader["unit"]?.ToString() ?? ""
                    });
                }
            }

            return batches.Values.ToList();
        }
        public List<Batch> GetByProductId(int productId)
        {
            var list = new List<Batch>();
            using var conn = _dbHelper.GetConnection();

            using var cmd = new SqlCommand("SP_Batch_GetByProductId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ProductId", productId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(BatchMapper.MapSimple(reader));
            }

            return list;
        }

        public List<Batch> GetByFarmId(int farmId)
        {
            var list = new List<Batch>();
            using var conn = _dbHelper.GetConnection();

            using var cmd = new SqlCommand("SP_Batch_GetByFarmId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FarmId", farmId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(BatchMapper.MapForFarm(reader));
            }

            return list;
        }

        public int Create(Batch batch)
        {
            using var conn = _dbHelper.GetConnection();

            const string sql = @"
                INSERT INTO dbo.batches
                (batch_code, product_id, farm_id, created_by_user_id,
                 harvest_date, quantity, unit, expiry_date, status, created_at)
                VALUES
                (@BatchCode, @ProductId, @FarmId, @CreatedByUserId,
                 @HarvestDate, @Quantity, @Unit, @ExpiryDate, @Status, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BatchCode", batch.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", batch.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", batch.FarmId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)batch.CreatedByUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HarvestDate", batch.HarvestDate?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", (object?)batch.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)batch.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", batch.ExpiryDate?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", batch.Status);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            _auditHelper.InsertLog(
                batch.CreatedByUserId,
                "INSERT",
                "batches",
                id.ToString(),
                null,
                batch
            );

            return id;
        }
        public bool Update(Batch batch)
        {
            var oldBatch = GetById(batch.Id);
            if (oldBatch == null) return false;

            using var conn = _dbHelper.GetConnection();

            const string sql = @"
                UPDATE dbo.batches
                SET batch_code = @BatchCode,
                    product_id = @ProductId,
                    farm_id = @FarmId,
                    harvest_date = @HarvestDate,
                    quantity = @Quantity,
                    unit = @Unit,
                    expiry_date = @ExpiryDate,
                    status = @Status
                WHERE id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", batch.Id);
            cmd.Parameters.AddWithValue("@BatchCode", batch.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", batch.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", batch.FarmId);
            cmd.Parameters.AddWithValue("@HarvestDate",
                batch.HarvestDate?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity",
                (object?)batch.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit",
                (object?)batch.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate",
                batch.ExpiryDate?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", batch.Status);

            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                _auditHelper.InsertLog(
                    batch.CreatedByUserId,
                    "UPDATE",
                    "batches",
                    batch.Id.ToString(),
                    oldBatch,
                    batch
                );
            }

            return ok;
        }
        public bool Delete(int id, int? deletedByUserId = null)
        {
            var oldBatch = GetById(id);
            if (oldBatch == null) return false;

            using var conn = _dbHelper.GetConnection();

            const string sql = "DELETE FROM dbo.batches WHERE id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                _auditHelper.InsertLog(
                    deletedByUserId,
                    "DELETE",
                    "batches",
                    id.ToString(),
                    oldBatch,
                    null
                );
            }

            return ok;
        }
        public int GetRetailerIdByUserId(int userId)
        {
            using var conn = _dbHelper.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT id FROM retailers WHERE user_id = @uid",
                conn
            );
            cmd.Parameters.AddWithValue("@uid", userId);

            return (int?)cmd.ExecuteScalar() ?? 0;
        }

        public bool UpdateStatus(int id, string status)
        {
            var oldBatch = GetById(id);
            if (oldBatch == null) return false;

            using var conn = _dbHelper.GetConnection();
            const string sql = "UPDATE dbo.batches SET status = @Status WHERE id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);

            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                oldBatch.Status = status;
                _auditHelper.InsertLog(
                    oldBatch.CreatedByUserId,
                    "UPDATE_STATUS",
                    "batches",
                    id.ToString(),
                    null,
                    oldBatch
                );
            }

            return ok;
        }
    }
}