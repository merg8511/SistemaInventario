using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Utilities;
using System.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                category.CState = true;
                return View(category);
            }

            category = await _unitOfWork.Category.Get(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    await _unitOfWork.Category.Add(category);
                    TempData[DS.Success] = "Categoria creada exitosamente";
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                    TempData[DS.Success] = "Categoria actualizada exitosamente";
                }
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar categoria";
            return View(category);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitOfWork.Category.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.Get(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Error al borrar la Categoria." });
            }

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Categoria eliminada con éxito" });
        }

        [ActionName("ValidationName")]
        public async Task<IActionResult> ValidationName(string name, int id = 0)
        {
            bool value = false;
            var categories = await _unitOfWork.Category.GetAll();
            if (id == 0)
            {
                value = categories.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim());
            }
            else
            {
                value = categories.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim() && w.Id != id);
            }

            if (value)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }
        #endregion
    }
}
