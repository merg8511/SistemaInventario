using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public Company Company { get; set; }
        public Product Product { get; set; }
        public int Stock { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public Order Order { get; set; }
    }
}
