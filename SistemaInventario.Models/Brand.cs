using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El nombre debe tener un máximo de {0} caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descripción es requerido")]
        [MaxLength(100, ErrorMessage = "La descripción debe tener un máximo de {0} caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Estado es requerido")]
        public bool BState{ get; set; }
    }
}
