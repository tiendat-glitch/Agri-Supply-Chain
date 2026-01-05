using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class WarehouseBusiness
    {
        private readonly WarehouseRepository _warehouseRepo;
        private readonly WarehouseStockRepository _stockRepo;

        public WarehouseBusiness(DatabaseHelper dbHelper)
        {
            _warehouseRepo = new WarehouseRepository(dbHelper);
            _stockRepo = new WarehouseStockRepository(dbHelper);
        }

        // Warehouses
        public List<WarehouseDTO> GetAllWarehouses()
        {
            return _warehouseRepo.GetAll().Select(MapWarehouse).ToList();
        }

        public WarehouseDTO? GetWarehouse(int id)
        {
            if (id <= 0) throw new Exception("WarehouseId không hợp lệ");
            var w = _warehouseRepo.GetById(id);
            return w == null ? null : MapWarehouse(w);
        }

        public int CreateWarehouse(CreateWarehouseDto dto)
        {
            if (dto == null) throw new Exception("Dữ liệu kho không được để trống");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Tên kho không được để trống");

            var entity = new Warehouse
            {
                Name = dto.Name,
                Location = dto.Location,
                ContactInfo = dto.ContactInfo,
                IsActive = dto.IsActive
            };

            return _warehouseRepo.Create(entity);
        }

        public bool UpdateWarehouse(int id, UpdateWarehouseDto dto)
        {
            if (id <= 0) throw new Exception("WarehouseId không hợp lệ");
            if (dto == null) throw new Exception("Dữ liệu kho không được để trống");

            var existing = _warehouseRepo.GetById(id) ?? throw new Exception("Kho không tồn tại");

            if (!string.IsNullOrWhiteSpace(dto.Name)) existing.Name = dto.Name!;
            if (dto.Location != null) existing.Location = dto.Location;
            if (dto.ContactInfo != null) existing.ContactInfo = dto.ContactInfo;
            if (dto.IsActive.HasValue) existing.IsActive = dto.IsActive.Value;

            return _warehouseRepo.Update(id, existing);
        }

        public bool DeleteWarehouse(int id)
        {
            if (id <= 0) throw new Exception("WarehouseId không hợp lệ");
            return _warehouseRepo.Delete(id);
        }

        // Stocks
        public List<WarehouseStockDTO> GetStocks(int warehouseId)
        {
            if (warehouseId <= 0) throw new Exception("WarehouseId không hợp lệ");
            return _stockRepo.GetByWarehouse(warehouseId).Select(MapStock).ToList();
        }

        public WarehouseStockDTO AdjustStock(AdjustWarehouseStockDto dto)
        {
            if (dto == null) throw new Exception("Dữ liệu stock không được để trống");
            if (dto.WarehouseId <= 0) throw new Exception("WarehouseId không hợp lệ");
            if (dto.BatchId <= 0) throw new Exception("BatchId không hợp lệ");
            if (dto.QuantityDelta == 0) throw new Exception("QuantityDelta phải khác 0");

            var entity = _stockRepo.Upsert(dto.WarehouseId, dto.BatchId, dto.QuantityDelta, dto.Unit);
            return MapStock(entity);
        }

        private static WarehouseDTO MapWarehouse(Warehouse w)
        {
            return new WarehouseDTO
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                ContactInfo = w.ContactInfo,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt
            };
        }

        private static WarehouseStockDTO MapStock(WarehouseStock s)
        {
            return new WarehouseStockDTO
            {
                Id = s.Id,
                WarehouseId = s.WarehouseId,
                BatchId = s.BatchId,
                Quantity = s.Quantity,
                Unit = s.Unit,
                LastUpdated = s.LastUpdated
            };
        }
    }
}


