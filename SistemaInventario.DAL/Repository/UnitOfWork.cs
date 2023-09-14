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

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Warehouse = new WarehouseRepository(_dbContext);
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
