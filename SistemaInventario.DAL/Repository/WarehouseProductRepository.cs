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
    public class WarehouseProductRepository : Repository<WarehouseProduct>, IWarehouseProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public WarehouseProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(WarehouseProduct warehouseProduct)
        {
            var entity = _dbContext.WarehouseProducts.FirstOrDefault(w => w.Id == warehouseProduct.Id);

            if (entity != null)
            {
                entity.Quantity = warehouseProduct.Quantity;
                _dbContext.SaveChanges();
            }
        }

    }
}
