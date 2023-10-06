using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventory)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Product.GetAllDropDownList("Category"),
                BrandList = _unitOfWork.Product.GetAllDropDownList("Brand"),
                ParentList = _unitOfWork.Product.GetAllDropDownList("Product")
            };

            if (id == null)
            {
                //new product
                productVM.Product.pState = true;
                return View(productVM);
            }
            else
            {
                productVM.Product = await _unitOfWork.Product.Get(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    string upload = webRootPath + DS.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.UrlImage = fileName + extension;
                    productVM.Product.pState = true;
                    await _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    var objProduct = await _unitOfWork.Product.GetById(p => p.Id == productVM.Product.Id, isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var lastFile = Path.Combine(upload, objProduct.UrlImage);

                        if (System.IO.File.Exists(lastFile))
                        {
                            System.IO.File.Delete(lastFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.UrlImage = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.UrlImage = objProduct.UrlImage;
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                }

                TempData[DS.Success] = "Producto se grabó con exito!";
                await _unitOfWork.Save();
                return View("Index");
            }
            productVM.BrandList = _unitOfWork.Product.GetAllDropDownList("Brand");
            productVM.CategoryList = _unitOfWork.Product.GetAllDropDownList("Category");
            productVM.ParentList = _unitOfWork.Product.GetAllDropDownList("Product");

            return View(productVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.Get(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error al borrar el producto." });
            }

            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var lastFile = Path.Combine(upload, product.UrlImage);

            if (System.IO.File.Exists(lastFile))
            {
                System.IO.File.Delete(lastFile);
            }

            _unitOfWork.Product.Remove(product);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Producto eliminada con éxito" });
        }

        [ActionName("ValidationSerie")]
        public async Task<IActionResult> ValidationSerie(string serie, int id = 0)
        {
            bool value = false;
            var products = await _unitOfWork.Product.GetAll();
            if (id == 0)
            {
                value = products.Any(w => w.SerieNumber.ToLower().Trim() == serie.ToLower().Trim());
            }
            else
            {
                value = products.Any(w => w.SerieNumber.ToLower().Trim() == serie.ToLower().Trim() && w.Id != id);
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
