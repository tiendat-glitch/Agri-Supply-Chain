using System.Text.Json;
using BLL.DTO;
using DAL.Helper;
using DAL.Repositories;
using Model;

namespace BLL
{
    public class AuditLogBusiness
    {
        private readonly AuditLogRepository _repo;

        public AuditLogBusiness(DatabaseHelper dbHelper)
        {
            _repo = new AuditLogRepository(dbHelper);
        }

        public List<AuditLogDTO> GetLogs(AuditLogFilterDto filter)
        {
            var logs = _repo.GetAll(
                userId: filter.UserId,
                tableName: filter.TableName,
                action: filter.Action,
                from: filter.From,
                to: filter.To);
            return logs.Select(Map).ToList();
        }

        public int WriteLog(int? userId, string action, string? tableName, string? rowId, object? oldValue, object? newValue)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new Exception("Action không được để trống");

            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RowId = rowId,
                OldValue = oldValue == null ? null : JsonSerializer.Serialize(oldValue),
                NewValue = newValue == null ? null : JsonSerializer.Serialize(newValue)
            };

            return _repo.Insert(log);
        }


        private static AuditLogDTO Map(AuditLog e)
        {
            return new AuditLogDTO
            {
                Id = e.Id,
                UserId = e.UserId,
                Action = e.Action,
                TableName = e.TableName,
                RowId = e.RowId,
                OldValue = e.OldValue,
                NewValue = e.NewValue,
                CreatedAt = e.CreatedAt
            };
        }
    }
}

