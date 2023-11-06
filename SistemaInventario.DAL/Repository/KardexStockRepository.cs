using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DAL.Data;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository
{
    public class KardexStockRepository : Repository<KardexStock>, IKardexStockRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public KardexStockRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task KardexRecord(int warehouseProductId, string type, string detail,
            int lastStock, int quantity, string userId)
        {
            var warehouseProduct = await _dbContext.WarehouseProducts
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == warehouseProductId);

            if (type == "Input")
            {
                KardexStock kardex = new KardexStock();
                kardex.WarehouseProductId = warehouseProductId;
                kardex.Type = type;
                kardex.Detail = detail;
                kardex.LastStock = lastStock;
                kardex.Quantity = quantity;
                kardex.Cost = warehouseProduct.Product.Cost;
                kardex.Stock = lastStock + quantity;
                kardex.Total = kardex.Stock * kardex.Cost;
                kardex.AppUserId = userId;
                kardex.RecordDate = DateTime.Now;

                await _dbContext.KardexStocks.AddAsync(kardex);
                await _dbContext.SaveChangesAsync();
            }

            if (type == "Output")
            {
                KardexStock kardex = new KardexStock();
                kardex.WarehouseProductId = warehouseProductId;
                kardex.Type = type;
                kardex.Detail = detail;
                kardex.LastStock = lastStock;
                kardex.Quantity = quantity;
                kardex.Cost = warehouseProduct.Product.Cost;
                kardex.Stock = lastStock - quantity;
                kardex.Total = kardex.Stock * kardex.Cost;
                kardex.AppUserId = userId;
                kardex.RecordDate = DateTime.Now;

                await _dbContext.KardexStocks.AddAsync(kardex);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
