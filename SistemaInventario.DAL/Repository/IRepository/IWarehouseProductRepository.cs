﻿using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository.IRepository
{
    public interface IWarehouseProductRepository : IRepository<WarehouseProduct>
    {
        void Update(WarehouseProduct warehouseProduct);
    }
}
