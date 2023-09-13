using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombre es requerido")]
        [Display(Name = "Bodega")]
        [MaxLength(60, ErrorMessage = "Nombre debe ser máximo 60 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [Display(Name = "Descripción")]
        [MaxLength(100, ErrorMessage = "Nombre debe ser máximo 60 caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Estado es requerido")]
        [Display(Name = "Estado")]
        public bool WHState { get; set; }
    }
}
