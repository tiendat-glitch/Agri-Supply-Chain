using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class QrRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public QrRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public QrCode? GetByToken(string token)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_id, token, url, generated_at
                FROM dbo.qr_codes
                WHERE token = @Token";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Token", token);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public QrCode? GetByBatchId(int batchId)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_id, token, url, generated_at
                FROM dbo.qr_codes
                WHERE batch_id = @BatchId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BatchId", batchId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int UpsertForBatch(int batchId, string token, string url)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                IF EXISTS (SELECT 1 FROM dbo.qr_codes WHERE batch_id = @BatchId)
                BEGIN
                    UPDATE dbo.qr_codes
                    SET token = @Token, url = @Url, generated_at = SYSUTCDATETIME()
                    WHERE batch_id = @BatchId;
                    SELECT id FROM dbo.qr_codes WHERE batch_id = @BatchId;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.qr_codes (batch_id, token, url, generated_at)
                    VALUES (@BatchId, @Token, @Url, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();
                END";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BatchId", batchId);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@Url", (object?)url ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private static QrCode Map(SqlDataReader reader)
        {
            return new QrCode
            {
                Id = Convert.ToInt32(reader["id"]),
                BatchId = Convert.ToInt32(reader["batch_id"]),
                Token = reader["token"].ToString()!,
                Url = reader["url"] == DBNull.Value ? null : reader["url"].ToString(),
                GeneratedAt = Convert.ToDateTime(reader["generated_at"])
            };
        }
    }
}

