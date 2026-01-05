using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class ShipmentBusiness
    {
        private readonly ShipmentRepository _repo;
        private readonly ShipmentItemRepository _itemRepo;
        private readonly WarehouseBusiness _warehouseBusiness;

        public ShipmentBusiness(DatabaseHelper dbHelper)
        {
            _repo = new ShipmentRepository(dbHelper);
            _itemRepo = new ShipmentItemRepository(dbHelper);
            _warehouseBusiness = new WarehouseBusiness(dbHelper);
        }

        public List<ShipmentDTO> GetAll()
        {
            return _repo.GetAll().Select(Map).ToList();
        }

        public ShipmentDTO? GetById(int id)
        {
            if (id <= 0) throw new Exception("ShipmentId không hợp lệ");
            var e = _repo.GetById(id);
            return e == null ? null : Map(e);
        }

        public int CreateShipment(CreateShipmentDto dto, int plannerUserId)
        {
            if (dto == null) throw new Exception("Dữ liệu shipment không được để trống");
            if (string.IsNullOrWhiteSpace(dto.ShipmentCode))
                throw new Exception("ShipmentCode không được để trống");
            if (string.IsNullOrWhiteSpace(dto.FromType) || string.IsNullOrWhiteSpace(dto.ToType))
                throw new Exception("FromType/ToType không được để trống");

            var entity = new Shipment
            {
                ShipmentCode = dto.ShipmentCode,
                PlannedByUserId = plannerUserId,
                DistributorId = dto.DistributorId,
                FromType = dto.FromType,
                FromId = dto.FromId,
                ToType = dto.ToType,
                ToId = dto.ToId,
                VehicleInfo = dto.VehicleInfo,
                DriverInfo = dto.DriverInfo,
                DepartureTime = dto.DepartureTime,
                Status = "pending",
                Notes = dto.Notes
            };

            return _repo.Create(entity);
        }

        public bool UpdateShipment(int id, UpdateShipmentDto dto)
        {
            if (id <= 0) throw new Exception("ShipmentId không hợp lệ");
            var existing = _repo.GetById(id) ?? throw new Exception("Shipment không tồn tại");

            if (dto.DistributorId.HasValue) existing.DistributorId = dto.DistributorId.Value;
            if (dto.VehicleInfo != null) existing.VehicleInfo = dto.VehicleInfo;
            if (dto.DriverInfo != null) existing.DriverInfo = dto.DriverInfo;
            if (dto.DepartureTime.HasValue) existing.DepartureTime = dto.DepartureTime;
            if (dto.ArrivalTime.HasValue) existing.ArrivalTime = dto.ArrivalTime;
            if (dto.Notes != null) existing.Notes = dto.Notes;

            return _repo.Update(id, existing);
        }

        public bool DeleteShipment(int id)
        {
            if (id <= 0) throw new Exception("ShipmentId không hợp lệ");
            return _repo.Delete(id);
        }

        public bool UpdateStatus(int id, string status)
        {
            if (id <= 0) throw new Exception("ShipmentId không hợp lệ");
            if (string.IsNullOrWhiteSpace(status)) throw new Exception("Status không hợp lệ");

            var shipment = _repo.GetById(id) ?? throw new Exception("Shipment không tồn tại");

            // Nếu trạng thái đến nơi -> cập nhật tồn kho
            if (status == "arrived")
            {
                var items = _itemRepo.GetByShipment(id);
                foreach (var item in items)
                {
                    // Trừ kho nguồn nếu nguồn là warehouse
                    if (shipment.FromType == "warehouse" && shipment.FromId.HasValue)
                    {
                        _warehouseBusiness.AdjustStock(new AdjustWarehouseStockDto
                        {
                            WarehouseId = shipment.FromId.Value,
                            BatchId = item.BatchId,
                            QuantityDelta = -item.Quantity,
                            Unit = item.Unit
                        });
                    }
                    // Cộng kho đích nếu đích là warehouse
                    if (shipment.ToType == "warehouse" && shipment.ToId.HasValue)
                    {
                        _warehouseBusiness.AdjustStock(new AdjustWarehouseStockDto
                        {
                            WarehouseId = shipment.ToId.Value,
                            BatchId = item.BatchId,
                            QuantityDelta = item.Quantity,
                            Unit = item.Unit
                        });
                    }
                }
            }

            return _repo.UpdateStatus(id, status, status == "arrived" ? DateTime.UtcNow : null);
        }

        // Items
        public List<ShipmentItemDTO> GetItems(int shipmentId)
        {
            return _itemRepo.GetByShipment(shipmentId).Select(MapItem).ToList();
        }

        public int AddItem(int shipmentId, CreateShipmentItemDto dto)
        {
            if (dto == null) throw new Exception("Dữ liệu item không được để trống");
            if (dto.BatchId <= 0) throw new Exception("BatchId không hợp lệ");
            if (dto.Quantity <= 0) throw new Exception("Quantity phải > 0");

            var entity = new ShipmentItem
            {
                ShipmentId = shipmentId,
                BatchId = dto.BatchId,
                Quantity = dto.Quantity,
                Unit = dto.Unit
            };
            return _itemRepo.Create(entity);
        }

        public bool UpdateItem(int itemId, CreateShipmentItemDto dto)
        {
            if (itemId <= 0) throw new Exception("ItemId không hợp lệ");
            var entity = new ShipmentItem
            {
                Id = itemId,
                BatchId = dto.BatchId,
                Quantity = dto.Quantity,
                Unit = dto.Unit
            };
            return _itemRepo.Update(itemId, entity);
        }

        public bool DeleteItem(int itemId)
        {
            if (itemId <= 0) throw new Exception("ItemId không hợp lệ");
            return _itemRepo.Delete(itemId);
        }

        private static ShipmentDTO Map(Shipment e)
        {
            return new ShipmentDTO
            {
                Id = e.Id,
                ShipmentCode = e.ShipmentCode,
                PlannedByUserId = e.PlannedByUserId,
                DistributorId = e.DistributorId,
                FromType = e.FromType,
                FromId = e.FromId,
                ToType = e.ToType,
                ToId = e.ToId,
                VehicleInfo = e.VehicleInfo,
                DriverInfo = e.DriverInfo,
                DepartureTime = e.DepartureTime,
                ArrivalTime = e.ArrivalTime,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                Notes = e.Notes
            };
        }

        private static ShipmentItemDTO MapItem(ShipmentItem i)
        {
            return new ShipmentItemDTO
            {
                Id = i.Id,
                ShipmentId = i.ShipmentId,
                BatchId = i.BatchId,
                Quantity = i.Quantity,
                Unit = i.Unit
            };
        }
    }
}

