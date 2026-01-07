using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL.Helper
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ================= EXISTING =================

        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public SqlDataReader ExecuteStoredProcedure(
            string procedureName,
            SqlConnection conn,
            List<SqlParameter>? parameters = null)
        {
            var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

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

        public object? ExecuteScalar(
            string sql,
            List<SqlParameter>? parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteScalar();
        }

        public SqlDataReader ExecuteReaderText(
            string sql,
            SqlConnection conn,
            List<SqlParameter>? parameters = null)
        {
            var cmd = new SqlCommand(sql, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
