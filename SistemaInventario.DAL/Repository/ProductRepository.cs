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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Product product)
        {
            var entity = _dbContext.Products.FirstOrDefault(w => w.Id == product.Id);

            if (entity != null)
            {
                if (product.UrlImage != null)
                {
                    product.UrlImage = product.UrlImage;
                }
                entity.SerieNumber = product.SerieNumber;
                entity.Description = product.Description;
                entity.Price = product.Price;
                entity.Cost = product.Cost;
                entity.CategoryId = product.CategoryId;
                entity.BrandId = product.BrandId;
                entity.ParentId = product.ParentId;
                entity.pState = product.pState;

                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if (obj == "Category")
            {
                return _dbContext.Categories.Where(c => c.CState == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            if (obj == "Brand")
            {
                return _dbContext.Brands.Where(b => b.BState == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            if (obj == "Product")
            {
                return _dbContext.Products.Where(b => b.pState == true).Select(c => new SelectListItem
                {
                    Text = c.Description,
                    Value = c.Id.ToString()
                });
            }

            return null;
        }
    }
}
