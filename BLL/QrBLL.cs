using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;

namespace BLL
{
    public class QrBusiness
    {
        private readonly QrRepository _qrRepo;

        public QrBusiness(DatabaseHelper dbHelper)
        {
            _qrRepo = new QrRepository(dbHelper);
        }

        public QrCodeDTO GenerateForBatch(int batchId, string baseUrl)
        {
            if (batchId <= 0) throw new Exception("BatchId không hợp lệ");
            var token = Guid.NewGuid().ToString("N");
            var url = $"{baseUrl.TrimEnd('/')}/api/qr/trace/{token}";
            var id = _qrRepo.UpsertForBatch(batchId, token, url);
            var qr = _qrRepo.GetByBatchId(batchId)!;
            return Map(qr);
        }

        public QrCodeDTO? GetByToken(string token)
        {
            var qr = _qrRepo.GetByToken(token);
            return qr == null ? null : Map(qr);
        }

        private static QrCodeDTO Map(Model.QrCode qr)
        {
            return new QrCodeDTO
            {
                Id = qr.Id,
                BatchId = qr.BatchId,
                Token = qr.Token,
                Url = qr.Url,
                GeneratedAt = qr.GeneratedAt
            };
        }
    }

    public class TraceBusiness
    {
        private readonly QrRepository _qrRepo;
        private readonly BatchRepository _batchRepo;
        private readonly FarmRepository _farmRepo;
        private readonly ProductRepository _productRepo;
        private readonly InspectionRepository _inspectionRepo;
        private readonly ShipmentRepository _shipmentRepo;
        private readonly ShipmentItemRepository _shipmentItemRepo;

        public TraceBusiness(DatabaseHelper dbHelper)
        {
            _qrRepo = new QrRepository(dbHelper);
            _batchRepo = new BatchRepository(dbHelper);
            _farmRepo = new FarmRepository(dbHelper);
            _productRepo = new ProductRepository(dbHelper);
            _inspectionRepo = new InspectionRepository(dbHelper);
            _shipmentRepo = new ShipmentRepository(dbHelper);
            _shipmentItemRepo = new ShipmentItemRepository(dbHelper);
        }

        public TraceResultDto TraceByToken(string token)
        {
            var qr = _qrRepo.GetByToken(token) ?? throw new Exception("QR không tồn tại");
            var batch = _batchRepo.GetById(qr.BatchId) ?? throw new Exception("Batch không tồn tại");
            var farm = _farmRepo.GetById(batch.FarmId) ?? throw new Exception("Farm không tồn tại");
            var product = _productRepo.GetById(batch.ProductId) ?? throw new Exception("Product không tồn tại");

            var inspections = _inspectionRepo.GetAll().Where(i => i.BatchId == batch.Id).ToList();

            // Lấy các shipment có item chứa batch này
            var shipments = _shipmentRepo.GetAll();
            var shipmentItems = _shipmentItemRepo.GetByShipmentIds(shipments.Select(s => s.Id).ToList());
            var shipmentIdsContainsBatch = shipmentItems
                .Where(i => i.BatchId == batch.Id)
                .Select(i => i.ShipmentId)
                .Distinct()
                .ToHashSet();
            var relatedShipments = shipments.Where(s => shipmentIdsContainsBatch.Contains(s.Id)).ToList();

            return new TraceResultDto
            {
                Batch = Map(batch),
                Farm = Map(farm),
                Product = Map(product),
                Inspections = inspections.Select(Map).ToList(),
                Shipments = relatedShipments.Select(Map).ToList()
            };
        }

        private static BatchDTO Map(Model.Batch b)
        {
            return new BatchDTO
            {
                Id = b.Id,
                BatchCode = b.BatchCode,
                ProductId = b.ProductId,
                FarmId = b.FarmId,
                CreatedByUserId = b.CreatedByUserId,
                HarvestDate = b.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                Quantity = b.Quantity,
                Unit = b.Unit,
                ExpiryDate = b.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = b.Status,
                CreatedAt = b.CreatedAt
            };
        }

        private static FarmDTO Map(Model.Farm f)
        {
            return new FarmDTO
            {
                Id = f.Id,
                Name = f.Name,
                OwnerName = f.OwnerName,
                Location = f.Location,
                ContactInfo = f.ContactInfo,
                Certifications = f.Certifications,
                CreatedAt = f.CreatedAt
            };
        }

        private static ProductDTO Map(Model.Product p)
        {
            return new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Category = p.Category,
                StorageInstructions = p.StorageInstructions,
                TypicalShelfLifeDays = p.TypicalShelfLifeDays,
                CreatedAt = p.CreatedAt
            };
        }

        private static InspectionDTO Map(Model.Inspection i)
        {
            return new InspectionDTO
            {
                Id = i.Id,
                BatchId = i.BatchId,
                InspectorUserId = i.InspectorUserId,
                InspectionDate = i.InspectionDate,
                Humidity = i.Humidity,
                Temperature = i.Temperature,
                ChemicalResidue = i.ChemicalResidue,
                QualityScore = i.QualityScore,
                ReportFile = i.ReportFile,
                SignatureId = i.SignatureId,
                Notes = i.Notes
            };
        }

        private static ShipmentDTO Map(Model.Shipment s)
        {
            return new ShipmentDTO
            {
                Id = s.Id,
                ShipmentCode = s.ShipmentCode,
                PlannedByUserId = s.PlannedByUserId,
                DistributorId = s.DistributorId,
                FromType = s.FromType,
                FromId = s.FromId,
                ToType = s.ToType,
                ToId = s.ToId,
                VehicleInfo = s.VehicleInfo,
                DriverInfo = s.DriverInfo,
                DepartureTime = s.DepartureTime,
                ArrivalTime = s.ArrivalTime,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                Notes = s.Notes
            };
        }
    }
}

