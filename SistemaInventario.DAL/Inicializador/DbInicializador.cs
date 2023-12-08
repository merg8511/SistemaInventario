using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DAL.Data;
using SistemaInventario.Models;
using SistemaInventario.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Inicializador
{
    public class DbInicializador : IDbInicializador
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInicializador(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate(); //Ejecuta migraciones pendientes
                }
            }
            catch (Exception ex)
            {

            }

            //Datos iniciales
            if (_db.Roles.Any(r => r.Name == DS.Role_Admin)) return;

            _roleManager.CreateAsync(new IdentityRole(DS.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Client)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Inventory)).GetAwaiter().GetResult();

            //Crear un usuario administrador
            _userManager.CreateAsync(new AppUser
            {
                UserName = "merg8511@gmail.com",
                Email = "merg8511@gmail.com",
                EmailConfirmed = true,
                Names = "Mario",
                LastName = "Rodriguez"
            }, "Admin123!").GetAwaiter().GetResult();

            //asignar rol al usuario
            AppUser user = _db.AppUsers.Where(u => u.UserName == "merg8511@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(user, DS.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
