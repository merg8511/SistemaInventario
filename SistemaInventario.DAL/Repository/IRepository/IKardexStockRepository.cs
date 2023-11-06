using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository.IRepository
{
    public interface IKardexStockRepository : IRepository<KardexStock>
    {
        Task KardexRecord(int WarehouseProductId, string type, string detail, int lastStock, int quantity, string userId);
    }
}
