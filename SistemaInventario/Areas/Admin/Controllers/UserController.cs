using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DAL.Data;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Utilities;
using System.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class UserController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbcontext;

        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbcontext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var users = await _unitOfWork.AppUser.GetAll();
            var userRol = await _dbcontext.UserRoles.ToListAsync();
            var roles = await _dbcontext.Roles.ToListAsync();

            foreach (var user in users)
            {
                var roleId = userRol.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> BlockAndUnBlock([FromBody] string id)
        {
            var user = await _unitOfWork.AppUser.GetById(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "Error de usuario" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Operacion exitosa" });
        }

        #endregion
    }
}
