using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "{0} es requerido")]
        [MaxLength(80)]
        public string Names { get; set; }

        [Required(ErrorMessage = "{0} es requerido")]
        [MaxLength(80)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} es requerido")]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} es requerido")]
        [MaxLength(60)]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} es requerido")]
        [MaxLength(60)]
        public string Country { get; set; }

        [NotMapped]
        public string Role { get; set; }

    }
}
