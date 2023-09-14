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
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public WarehouseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Warehouse warehouse)
        {
            var entity = _dbContext.Warehouses.FirstOrDefault(w => w.Id == warehouse.Id);

            if (entity != null)
            {
                entity.Name = warehouse.Name;
                entity.Description = warehouse.Description;
                entity.WHState = warehouse.WHState;
                _dbContext.SaveChanges();
            }
        }
    }
}
