using DAL.Helper;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Model;

namespace BLL.Retailer
{
    public class RetailerBatchBusiness
    {
        private readonly BatchRepository _batchRepo;

        public RetailerBatchBusiness(IConfiguration config)
        {
            string connStr = config.GetConnectionString("DefaultConnection");
            var helper = new DatabaseHelper(connStr);
            _batchRepo = new BatchRepository(helper);
        }

        // Lấy tất cả batch mà retailer được xem
        public List<Batch> GetAllBatches()
        {
            return _batchRepo.GetAll();
        }
        //Truy xuất nguồn gốc
        public List<Batch> GetQrTrace(int retailerId)
        {
            return _batchRepo.GetQrTraceForRetailer(retailerId);
        }

        // Chi tiết batch
        public Batch GetBatchById(int id)
        {
            return _batchRepo.GetById(id)
                   ?? throw new Exception("Batch không tồn tại");
        }

        // Theo product
        public List<Batch> GetByProductId(int productId)
        {
            return _batchRepo.GetByProductId(productId);
        }

        // Theo farm
        public List<Batch> GetByFarmId(int farmId)
        {
            return _batchRepo.GetByFarmId(farmId);
        }
    }
}
