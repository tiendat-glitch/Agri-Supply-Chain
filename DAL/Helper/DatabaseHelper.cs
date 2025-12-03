using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DAL.Helper
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Lấy SqlConnection đã mở
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // Thực thi Stored Procedure và trả về SqlDataReader
        public SqlDataReader ExecuteStoredProcedure(
            string procedureName,
            SqlConnection conn,
            List<SqlParameter>? parameters = null)
        {
            // KHÔNG dùng "using" ở đây vì Cmd sẽ bị dispose trước DataReader → lỗi
            var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            // Reader đóng → Connection tự đóng
        }

        // Stored Procedure không trả về dữ liệu
        public int ExecuteNonQueryStoredProcedure(
            string procedureName,
            SqlConnection conn,
            List<SqlParameter>? parameters = null)
        {
            using var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return cmd.ExecuteNonQuery();
        }

        // Stored Procedure trả về DataTable
        public DataTable ExecuteDataTableStoredProcedure(
            string procedureName,
            List<SqlParameter>? parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();

            da.Fill(dt);

            return dt;
        }
    }
}
