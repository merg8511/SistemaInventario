using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Upsert()
        {
            CompanyVM companyVM = new CompanyVM()
            {
                Company = new Company(),
                Warehouses = _unitOfWork.Stock.GetAllDropdownList("Warehouse")
            };

            companyVM.Company = await _unitOfWork.Company.GetById();

            if (companyVM.Company == null)
            {
                companyVM.Company = new Company();
            }

            return View(companyVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompanyVM companyVM)
        {
            if (ModelState.IsValid)
            {
                TempData[DS.Success] = "Compania guardada exitosamente";
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (companyVM.Company.Id == 0) //Crear compañia
                {
                    companyVM.Company.CreatedById = claim.Value;
                    companyVM.Company.UpdatedById = claim.Value;
                    companyVM.Company.DateCreated = DateTime.Now;
                    companyVM.Company.DateUpdated = DateTime.Now;
                    await _unitOfWork.Company.Add(companyVM.Company);
                }
                else
                {
                    companyVM.Company.UpdatedById = claim.Value;
                    companyVM.Company.DateUpdated = DateTime.Now;
                    _unitOfWork.Company.Update(companyVM.Company);
                }

                await _unitOfWork.Save();
                return RedirectToAction("Index", "Home", new { area = "Inventory" });
            }

            TempData[DS.Error] = "Error al guardar la compañia";
            return View(companyVM);
        }
    }
}
