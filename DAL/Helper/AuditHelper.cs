using System;
using System.Text.Json;

namespace DAL.Helper
{
    public class AuditHelper
    {
        private readonly DatabaseHelper _db;

        public AuditHelper(DatabaseHelper db)
        {
            _db = db;
        }

        public void InsertLog(
            int? userId,
            string action,
            string tableName,
            string rowId,
            object? oldValue,
            object? newValue)
        {
            using var conn = _db.GetConnection();
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                @"INSERT INTO dbo.audit_logs 
                  (user_id, action, table_name, row_id, old_value, new_value, created_at)
                  VALUES (@user_id, @action, @table_name, @row_id, @old_value, @new_value, SYSUTCDATETIME())", conn);

            cmd.Parameters.AddWithValue("@user_id", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.Parameters.AddWithValue("@table_name", (object?)tableName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@row_id", (object?)rowId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@old_value", oldValue == null ? DBNull.Value : JsonSerializer.Serialize(oldValue));
            cmd.Parameters.AddWithValue("@new_value", newValue == null ? DBNull.Value : JsonSerializer.Serialize(newValue));

            cmd.ExecuteNonQuery();
        }
    }
}
