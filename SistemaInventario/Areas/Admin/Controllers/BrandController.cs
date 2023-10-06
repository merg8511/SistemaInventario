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
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Brand brand = new Brand();

            if (id == null)
            {
                brand.BState = true;
                return View(brand);
            }

            brand = await _unitOfWork.Brand.Get(id.GetValueOrDefault());

            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Brand brand)
        {
            if (ModelState.IsValid)
            {
                if (brand.Id == 0)
                {
                    await _unitOfWork.Brand.Add(brand);
                    TempData[DS.Success] = "Marca creada exitosamente";
                }
                else
                {
                    _unitOfWork.Brand.Update(brand);
                    TempData[DS.Success] = "Marca actualizada exitosamente";
                }
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar marca";
            return View(brand);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitOfWork.Brand.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _unitOfWork.Brand.Get(id);
            if (brand == null)
            {
                return Json(new { success = false, message = "Error al borrar la marca." });
            }

            _unitOfWork.Brand.Remove(brand);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Marca eliminada con éxito" });
        }

        [ActionName("ValidationName")]
        public async Task<IActionResult> ValidationName(string name, int id = 0)
        {
            bool value = false;
            var brands = await _unitOfWork.Brand.GetAll();
            if (id == 0)
            {
                value = brands.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim());
            }
            else
            {
                value = brands.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim() && w.Id != id);
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
