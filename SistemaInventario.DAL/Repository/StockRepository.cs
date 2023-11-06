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
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public StockRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == "Warehouse")
            {
                return _dbContext.Warehouses.Where(x => x.WHState == true).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }); ;
            }
            return null;
        }

        public void Update(Stock stock)
        {
            var entity = _dbContext.Stocks.FirstOrDefault(w => w.Id == stock.Id);

            if (entity != null)
            {
                entity.WarehouseId = stock.WarehouseId;
                entity.FinalDate = stock.FinalDate;
                entity.State = stock.State;

                _dbContext.SaveChanges();
            }
        }

    }
}
