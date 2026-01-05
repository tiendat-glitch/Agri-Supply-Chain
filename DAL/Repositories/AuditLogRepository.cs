using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class AuditLogRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public AuditLogRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<AuditLog> GetAll(
            int? userId = null,
            string? tableName = null,
            string? action = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            var list = new List<AuditLog>();
            using SqlConnection conn = _dbHelper.GetConnection();

            var sql = new System.Text.StringBuilder(@"
                SELECT id, user_id, action, table_name, row_id, old_value, new_value, created_at
                FROM dbo.audit_logs
                WHERE 1=1
                ORDER BY created_at DESC
            ");

            var cmd = new SqlCommand();
            cmd.Connection = conn;

            if (userId.HasValue)
            {
                sql.Append(" AND user_id = @UserId");
                cmd.Parameters.AddWithValue("@UserId", userId.Value);
            }
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                sql.Append(" AND table_name = @TableName");
                cmd.Parameters.AddWithValue("@TableName", tableName);
            }
            if (!string.IsNullOrWhiteSpace(action))
            {
                sql.Append(" AND action = @Action");
                cmd.Parameters.AddWithValue("@Action", action);
            }
            if (from.HasValue)
            {
                sql.Append(" AND created_at >= @From");
                cmd.Parameters.AddWithValue("@From", from.Value);
            }
            if (to.HasValue)
            {
                sql.Append(" AND created_at <= @To");
                cmd.Parameters.AddWithValue("@To", to.Value);
            }

            sql.Append(" ORDER BY created_at DESC");
            cmd.CommandText = sql.ToString();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public int Insert(AuditLog log)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.audit_logs
                    (user_id, action, table_name, row_id, old_value, new_value, created_at)
                VALUES
                    (@UserId, @Action, @TableName, @RowId, @OldValue, @NewValue, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", (object?)log.UserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Action", log.Action);
            cmd.Parameters.AddWithValue("@TableName", (object?)log.TableName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RowId", (object?)log.RowId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OldValue", (object?)log.OldValue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NewValue", (object?)log.NewValue ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private static AuditLog Map(SqlDataReader reader)
        {
            return new AuditLog
            {
                Id = Convert.ToInt32(reader["id"]),
                UserId = reader["user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["user_id"]),
                Action = reader["action"].ToString()!,
                TableName = reader["table_name"] == DBNull.Value ? null : reader["table_name"].ToString(),
                RowId = reader["row_id"] == DBNull.Value ? null : reader["row_id"].ToString(),
                OldValue = reader["old_value"] == DBNull.Value ? null : reader["old_value"].ToString(),
                NewValue = reader["new_value"] == DBNull.Value ? null : reader["new_value"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}

