using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class KardexStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WarehouseProductId { get; set; }

        [ForeignKey("WarehouseProductId")]
        public WarehouseProduct WarehouseProduct { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        public string Detail { get; set; }

        [Required]
        public int LastStock { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Cost { get; set; }

        [Required]
        public int Stock { get; set; }

        public double Total { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        public DateTime RecordDate { get; set; }
    }
}
