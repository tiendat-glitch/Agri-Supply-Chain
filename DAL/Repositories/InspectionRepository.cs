using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class InspectionRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public InspectionRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Inspection> GetAll()
        {
            var list = new List<Inspection>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_id, inspector_user_id, inspection_date,
                       humidity, temperature, chemical_residue, quality_score,
                       report_file, signature_id, notes
                FROM dbo.inspections";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Inspection? GetById(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_id, inspector_user_id, inspection_date,
                       humidity, temperature, chemical_residue, quality_score,
                       report_file, signature_id, notes
                FROM dbo.inspections
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int Create(Inspection entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.inspections
                    (batch_id, inspector_user_id, inspection_date,
                     humidity, temperature, chemical_residue, quality_score,
                     report_file, signature_id, notes)
                VALUES
                    (@BatchId, @InspectorUserId, @InspectionDate,
                     @Humidity, @Temperature, @ChemicalResidue, @QualityScore,
                     @ReportFile, @SignatureId, @Notes);
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BatchId", entity.BatchId);
            cmd.Parameters.AddWithValue("@InspectorUserId", (object?)entity.InspectorUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InspectionDate", entity.InspectionDate);
            cmd.Parameters.AddWithValue("@Humidity", (object?)entity.Humidity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Temperature", (object?)entity.Temperature ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChemicalResidue", (object?)entity.ChemicalResidue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QualityScore", (object?)entity.QualityScore ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportFile", (object?)entity.ReportFile ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SignatureId", (object?)entity.SignatureId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)entity.Notes ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public bool Update(int id, Inspection entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.inspections
                SET humidity = @Humidity,
                    temperature = @Temperature,
                    chemical_residue = @ChemicalResidue,
                    quality_score = @QualityScore,
                    report_file = @ReportFile,
                    signature_id = @SignatureId,
                    notes = @Notes
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Humidity", (object?)entity.Humidity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Temperature", (object?)entity.Temperature ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChemicalResidue", (object?)entity.ChemicalResidue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@QualityScore", (object?)entity.QualityScore ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReportFile", (object?)entity.ReportFile ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SignatureId", (object?)entity.SignatureId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)entity.Notes ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.inspections WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Inspection Map(SqlDataReader reader)
        {
            return new Inspection
            {
                Id = Convert.ToInt32(reader["id"]),
                BatchId = Convert.ToInt32(reader["batch_id"]),
                InspectorUserId = reader["inspector_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["inspector_user_id"]),
                InspectionDate = Convert.ToDateTime(reader["inspection_date"]),
                Humidity = reader["humidity"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["humidity"]),
                Temperature = reader["temperature"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["temperature"]),
                ChemicalResidue = reader["chemical_residue"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["chemical_residue"]),
                QualityScore = reader["quality_score"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["quality_score"]),
                ReportFile = reader["report_file"] == DBNull.Value ? null : reader["report_file"].ToString(),
                SignatureId = reader["signature_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["signature_id"]),
                Notes = reader["notes"] == DBNull.Value ? null : reader["notes"].ToString()
            };
        }
    }
}


