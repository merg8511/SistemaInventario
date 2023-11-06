using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        [Required]
        public DateTime InitDate { get; set; }

        [Required]
        public DateTime FinalDate { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        [Required]
        public bool State { get; set; }

    }
}
