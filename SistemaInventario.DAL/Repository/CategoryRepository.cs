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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Category category)
        {
            var entity = _dbContext.Categories.FirstOrDefault(w => w.Id == category.Id);

            if (entity != null)
            {
                entity.Name = category.Name;
                entity.Description = category.Description;
                entity.CState = category.CState;
                _dbContext.SaveChanges();
            }
        }
    }
}
