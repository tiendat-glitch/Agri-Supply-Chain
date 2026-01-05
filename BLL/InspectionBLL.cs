using System.Security.Claims;
using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class InspectionBusiness
    {
        private readonly InspectionRepository _repo;

        public InspectionBusiness(DatabaseHelper dbHelper)
        {
            _repo = new InspectionRepository(dbHelper);
        }

        public List<InspectionDTO> GetAll()
        {
            return _repo.GetAll().Select(Map).ToList();
        }

        public InspectionDTO? GetById(int id)
        {
            if (id <= 0) throw new Exception("InspectionId không hợp lệ");
            var entity = _repo.GetById(id);
            return entity == null ? null : Map(entity);
        }

        public int Create(CreateInspectionDto dto, int? inspectorUserId)
        {
            if (dto == null) throw new Exception("Dữ liệu inspection không được để trống");
            if (dto.BatchId <= 0) throw new Exception("BatchId không hợp lệ");

            var entity = new Inspection
            {
                BatchId = dto.BatchId,
                InspectorUserId = inspectorUserId,
                InspectionDate = DateTime.UtcNow,
                Humidity = dto.Humidity,
                Temperature = dto.Temperature,
                ChemicalResidue = dto.ChemicalResidue,
                QualityScore = dto.QualityScore,
                ReportFile = dto.ReportFile,
                SignatureId = dto.SignatureId,
                Notes = dto.Notes
            };

            return _repo.Create(entity);
        }

        public bool Update(int id, UpdateInspectionDto dto)
        {
            if (id <= 0) throw new Exception("InspectionId không hợp lệ");
            var existing = _repo.GetById(id) ?? throw new Exception("Inspection không tồn tại");

            if (dto.Humidity.HasValue) existing.Humidity = dto.Humidity.Value;
            if (dto.Temperature.HasValue) existing.Temperature = dto.Temperature.Value;
            if (dto.ChemicalResidue.HasValue) existing.ChemicalResidue = dto.ChemicalResidue.Value;
            if (dto.QualityScore.HasValue) existing.QualityScore = dto.QualityScore.Value;
            if (dto.ReportFile != null) existing.ReportFile = dto.ReportFile;
            if (dto.SignatureId.HasValue) existing.SignatureId = dto.SignatureId.Value;
            if (dto.Notes != null) existing.Notes = dto.Notes;

            return _repo.Update(id, existing);
        }

        public bool Delete(int id)
        {
            if (id <= 0) throw new Exception("InspectionId không hợp lệ");
            return _repo.Delete(id);
        }

        private static InspectionDTO Map(Inspection e)
        {
            return new InspectionDTO
            {
                Id = e.Id,
                BatchId = e.BatchId,
                InspectorUserId = e.InspectorUserId,
                InspectionDate = e.InspectionDate,
                Humidity = e.Humidity,
                Temperature = e.Temperature,
                ChemicalResidue = e.ChemicalResidue,
                QualityScore = e.QualityScore,
                ReportFile = e.ReportFile,
                SignatureId = e.SignatureId,
                Notes = e.Notes
            };
        }
    }
}


