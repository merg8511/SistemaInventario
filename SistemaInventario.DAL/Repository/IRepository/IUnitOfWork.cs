﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IWarehouseRepository Warehouse { get; }
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        IAppUserRepository AppUser { get; }
        IWarehouseProductRepository WarehouseProduct { get; }
        IStockRepository Stock { get; }
        IStockDetailRepository StockDetail { get; }
        IKardexStockRepository KardexStock { get; }
        Task Save();
    }
}
