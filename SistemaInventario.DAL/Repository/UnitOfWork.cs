﻿using SistemaInventario.DAL.Data;
using SistemaInventario.DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IWarehouseRepository Warehouse { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IProductRepository Product { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IWarehouseProductRepository WarehouseProduct { get; private set; }
        public IStockRepository Stock { get; private set; }
        public IStockDetailRepository StockDetail { get; private set; }
        public IKardexStockRepository KardexStock { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Warehouse = new WarehouseRepository(_dbContext);
            Category = new CategoryRepository(_dbContext);
            Brand = new BrandRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
            AppUser = new AppUserRepository(_dbContext);
            WarehouseProduct = new WarehouseProductRepository(_dbContext);
            Stock = new StockRepository(_dbContext);
            StockDetail = new StockDetailRepository(_dbContext);
            KardexStock = new KardexStockRepository(_dbContext);
            Company = new CompanyRepository(_dbContext);
            ShoppingCart = new ShoppingCartRepository(_dbContext);
            Order = new OrderRepository(_dbContext);
            OrderDetail = new OrderDetailRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
