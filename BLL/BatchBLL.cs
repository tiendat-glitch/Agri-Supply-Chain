using System;
using System.Collections.Generic;
using System.Linq;
using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class BatchBusiness
    {
        private readonly BatchRepository _repo;
        private readonly AuditLogBusiness _auditBusiness;

        public BatchBusiness(DatabaseHelper dbHelper)
        {
            _repo = new BatchRepository(dbHelper);
            _auditBusiness = new AuditLogBusiness(dbHelper);
        }

        public List<BatchDTO> GetAll()
        {
            return _repo.GetAll().Select(Map).ToList();
        }

        public BatchDTO? GetById(int id)
        {
            if (id <= 0) throw new Exception("BatchId không hợp lệ");
            var batch = _repo.GetById(id);
            return batch == null ? null : Map(batch);
        }

        public int Create(CreateBatchDto dto, int? currentUserId = null)
        {
            if (dto == null) throw new Exception("Dữ liệu batch không được để trống");
            if (string.IsNullOrWhiteSpace(dto.BatchCode))
                throw new Exception("BatchCode không được để trống");

            var entity = new Batch
            {
                BatchCode = dto.BatchCode,
                ProductId = dto.ProductId,
                FarmId = dto.FarmId,
                CreatedByUserId = dto.CreatedByUserId ?? currentUserId,
                HarvestDate = dto.HarvestDate.HasValue ? DateOnly.FromDateTime(dto.HarvestDate.Value) : null,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                ExpiryDate = dto.ExpiryDate.HasValue ? DateOnly.FromDateTime(dto.ExpiryDate.Value) : null,
                Status = dto.Status ?? "pending"
            };

            var id = _repo.Create(entity);

            _auditBusiness.WriteLog(
                userId: currentUserId,
                action: "Create",
                tableName: "batches",
                rowId: id.ToString(),
                oldValue: null,
                newValue: entity
            );

            return id;
        }

        public bool Update(int id, UpdateBatchDto dto, int? currentUserId = null)
        {
            var existing = _repo.GetById(id)
                ?? throw new Exception("Batch không tồn tại");

            var oldValue = new Batch
            {
                Id = existing.Id,
                BatchCode = existing.BatchCode,
                ProductId = existing.ProductId,
                FarmId = existing.FarmId,
                CreatedByUserId = existing.CreatedByUserId,
                HarvestDate = existing.HarvestDate,
                Quantity = existing.Quantity,
                Unit = existing.Unit,
                ExpiryDate = existing.ExpiryDate,
                Status = existing.Status,
                CreatedAt = existing.CreatedAt
            };

            if (!string.IsNullOrWhiteSpace(dto.BatchCode))
                existing.BatchCode = dto.BatchCode;

            if (dto.ProductId.HasValue)
                existing.ProductId = dto.ProductId.Value;

            if (dto.FarmId.HasValue)
                existing.FarmId = dto.FarmId.Value;

            if (dto.CreatedByUserId.HasValue)
                existing.CreatedByUserId = dto.CreatedByUserId.Value;

            if (dto.HarvestDate.HasValue)
                existing.HarvestDate = DateOnly.FromDateTime(dto.HarvestDate.Value);

            if (dto.Quantity.HasValue)
                existing.Quantity = dto.Quantity.Value;

            if (!string.IsNullOrWhiteSpace(dto.Unit))
                existing.Unit = dto.Unit;

            if (dto.ExpiryDate.HasValue)
                existing.ExpiryDate = DateOnly.FromDateTime(dto.ExpiryDate.Value);

            if (!string.IsNullOrWhiteSpace(dto.Status))
                existing.Status = dto.Status;

            var ok = _repo.Update(existing);

            if (ok)
            {
                _auditBusiness.WriteLog(
                    userId: currentUserId,
                    action: "UPDATE",
                    tableName: "batches",
                    rowId: id.ToString(),
                    oldValue: oldValue,
                    newValue: existing
                );
            }

            return ok;
        }

        public bool Delete(int id, int? currentUserId = null)
        {
            var existing = _repo.GetById(id);
            var ok = _repo.Delete(id);

            if (ok && existing != null)
            {
                _auditBusiness.WriteLog(
                    userId: currentUserId,
                    action: "Delete",
                    tableName: "batches",
                    rowId: id.ToString(),
                    oldValue: existing,
                    newValue: null
                );
            }

            return ok;
        }
        public bool UpdateStatus(int id, string status, int? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Status không hợp lệ");

            var existing = _repo.GetById(id) ?? throw new Exception("Batch không tồn tại");
            var oldValue = existing;

            var ok = _repo.UpdateStatus(id, status);

            if (ok)
            {
                existing.Status = status;
                _auditBusiness.WriteLog(
                    userId: currentUserId,
                    action: "UpdateStatus",
                    tableName: "batches",
                    rowId: id.ToString(),
                    oldValue: oldValue,
                    newValue: existing
                );
            }

            return ok;
        }
        private static BatchDTO Map(Batch batch)
        {
            return new BatchDTO
            {
                Id = batch.Id,
                BatchCode = batch.BatchCode,
                ProductId = batch.ProductId,
                FarmId = batch.FarmId,
                CreatedByUserId = batch.CreatedByUserId,
                HarvestDate = batch.HarvestDate?.ToDateTime(TimeOnly.MinValue),
                Quantity = batch.Quantity,
                Unit = batch.Unit,
                ExpiryDate = batch.ExpiryDate?.ToDateTime(TimeOnly.MinValue),
                Status = batch.Status,
                CreatedAt = batch.CreatedAt
            };
        }
    }
}
