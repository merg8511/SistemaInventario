using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class StockDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StockId { get; set; }

        [ForeignKey("StockId")]
        public Stock Stock { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int LastStock { get; set; }

        public int Quantity { get; set; }
    }
}
