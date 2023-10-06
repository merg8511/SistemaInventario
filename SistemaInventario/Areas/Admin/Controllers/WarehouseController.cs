using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Utilities;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class WarehouseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Warehouse warehouse = new Warehouse();

            if (id == null)
            {
                warehouse.WHState = true;
                return View(warehouse);
            }

            warehouse = await _unitOfWork.Warehouse.Get(id.GetValueOrDefault());

            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                if (warehouse.Id == 0)
                {
                    await _unitOfWork.Warehouse.Add(warehouse);
                    TempData[DS.Success] = "Bodega creada exitosamente";
                }
                else
                {
                    _unitOfWork.Warehouse.Update(warehouse);
                    TempData[DS.Success] = "Bodega actualizada exitosamente";
                }
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar bodega";
            return View(warehouse);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitOfWork.Warehouse.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _unitOfWork.Warehouse.Get(id);
            if (warehouse == null)
            {
                return Json(new { success = false, message = "Error al borrar la bodega." });
            }

            _unitOfWork.Warehouse.Remove(warehouse);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Bodega eliminada con éxito" });
        }

        [ActionName("ValidationName")]
        public async Task<IActionResult> ValidationName(string name, int id = 0)
        {
            bool value = false;
            var warehouses = await _unitOfWork.Warehouse.GetAll();
            if (id == 0)
            {
                value = warehouses.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim());
            }
            else
            {
                value = warehouses.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim() && w.Id != id);
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
