using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class FarmBusiness
    {
        private readonly FarmRepository _repo;

        public FarmBusiness(DatabaseHelper dbHelper)
        {
            _repo = new FarmRepository(dbHelper);
        }

        public List<FarmDTO> GetAll()
        {
            return _repo.GetAll().Select(Map).ToList();
        }

        public FarmDTO? GetById(int id)
        {
            if (id <= 0) throw new Exception("FarmId không hợp lệ");
            var farm = _repo.GetById(id);
            return farm == null ? null : Map(farm);
        }

        public int Create(CreateFarmDto dto)
        {
            if (dto == null) throw new Exception("Dữ liệu farm không được để trống");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Tên farm không được để trống");

            var entity = new Farm
            {
                Name = dto.Name,
                OwnerName = dto.OwnerName,
                Location = dto.Location,
                ContactInfo = dto.ContactInfo,
                Certifications = dto.Certifications
            };

            return _repo.Create(entity);
        }

        public bool Update(int id, UpdateFarmDto dto)
        {
            if (id <= 0) throw new Exception("FarmId không hợp lệ");
            if (dto == null) throw new Exception("Dữ liệu farm không được để trống");

            var existing = _repo.GetById(id) ?? throw new Exception("Farm không tồn tại");
            if (!string.IsNullOrWhiteSpace(dto.Name))
                existing.Name = dto.Name;
            if (dto.OwnerName != null)
                existing.OwnerName = dto.OwnerName;
            if (dto.Location != null)
                existing.Location = dto.Location;
            if (dto.ContactInfo != null)
                existing.ContactInfo = dto.ContactInfo;
            if (dto.Certifications != null)
                existing.Certifications = dto.Certifications;

            return _repo.Update(id, existing);
        }

        public bool Delete(int id)
        {
            if (id <= 0) throw new Exception("FarmId không hợp lệ");
            return _repo.Delete(id);
        }

        private static FarmDTO Map(Farm farm)
        {
            return new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name,
                OwnerName = farm.OwnerName,
                Location = farm.Location,
                ContactInfo = farm.ContactInfo,
                Certifications = farm.Certifications,
                CreatedAt = farm.CreatedAt
            };
        }
    }
}


