using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models.ViewModels
{
    public class StockVM
    {
        public Stock Stock { get; set; }
        public StockDetail StockDetail { get; set; }
        public IEnumerable<StockDetail> Details { get; set; }
        public IEnumerable<SelectListItem> Warehouses { get; set; }
    }
}
