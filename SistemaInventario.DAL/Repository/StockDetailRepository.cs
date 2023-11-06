using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class StockDetailRepository : Repository<StockDetail>, IStockDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public StockDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(StockDetail detail)
        {
            var entity = _dbContext.StockDetails.FirstOrDefault(w => w.Id == detail.Id);

            if (entity != null)
            {
                entity.LastStock = detail.LastStock;
                entity.Quantity = detail.Quantity;

                _dbContext.SaveChanges();
            }
        }

    }
}
