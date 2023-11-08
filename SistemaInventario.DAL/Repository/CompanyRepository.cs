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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Company company)
        {
            var entity = _dbContext.Companies.FirstOrDefault(w => w.Id == company.Id);

            if (entity != null)
            {
                entity.Name = company.Name;
                entity.Description = company.Description;
                company.Country = company.Country;
                company.City = company.City;
                company.Address = company.Address;
                company.Phone = company.Phone;
                company.WarehouseSaleId = company.WarehouseSaleId;
                company.UpdatedById = company.UpdatedById;
                company.DateUpdated = company.DateUpdated;

                _dbContext.SaveChanges();
            }
        }
    }
}
