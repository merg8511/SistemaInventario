using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models.ViewModels
{
    public class KardexStockVM
    {
        public Product Product { get; set; }
        public IEnumerable<KardexStock> KardexStocks { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime FinalDate { get; set; }
    }
}
