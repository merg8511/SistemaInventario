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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BrandRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Brand brand)
        {
            var entity = _dbContext.Brands.FirstOrDefault(w => w.Id == brand.Id);

            if (entity != null)
            {
                entity.Name = brand.Name;
                entity.Description = brand.Description;
                entity.BState = brand.BState;
                _dbContext.SaveChanges();
            }
        }
    }
}
