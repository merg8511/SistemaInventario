using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(60)]
        public string Country { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(60)]
        public string City { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(40)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int WarehouseSaleId { get; set; }

        [ForeignKey("WarehouseSaleId")]
        public Warehouse Warehouse { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public AppUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }

        [ForeignKey("UpdatedById")]
        public AppUser UpdatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

    }
}
