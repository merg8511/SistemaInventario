using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.Specifications;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM cart { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string search = "", string currentSearch = "")
        {

            // Controlar la sesion
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cartList = await _unitOfWork.ShoppingCart.GetAll(c => c.AppUserId == claim.Value);
                var numberProducts = cartList.Count();
                HttpContext.Session.SetInt32(DS.ssShoppingCart, numberProducts);
            }

            if (!String.IsNullOrEmpty(search))
            {
                pageNumber = 1;
            }
            else
            {
                search = currentSearch;
            }
            ViewData["CurrentSearch"] = search;

            if (pageNumber < 1) { pageNumber = 1; }

            Params param = new Params()
            {
                PageNumber = pageNumber,
                PageSize = 4
            };

            var result = _unitOfWork.Product.GetAllPagination(param);

            if (!String.IsNullOrEmpty(search))
            {
                result = _unitOfWork.Product.GetAllPagination(param, p => p.Description.Contains(search));
            }

            ViewData["TotalPages"] = result.MetaData.TotalPages;
            ViewData["TotalRecords"] = result.MetaData.TotalCount;
            ViewData["PageSize"] = result.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previus"] = "disabled";
            ViewData["Next"] = "";

            if (pageNumber > 1) { ViewData["Previus"] = ""; }
            if (result.MetaData.TotalPages <= pageNumber) { ViewData["Next"] = "disabled"; }

            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            cart = new ShoppingCartVM();

            cart.Company = await _unitOfWork.Company.GetById();
            cart.Product = await _unitOfWork.Product.GetById(p => p.Id == id,
                includeProperties: "Brand,Category");

            var warehouseProduct = await _unitOfWork.WarehouseProduct.GetById(w => w.ProductId == id &&
            w.WarehouseId == cart.Company.WarehouseSaleId);

            if (warehouseProduct == null)
            {
                cart.Stock = 0;
            }
            else
            {
                cart.Stock = warehouseProduct.Quantity;
            }

            cart.ShoppingCart = new ShoppingCart()
            {
                Product = cart.Product,
                ProductId = cart.Product.Id
            };

            return View(cart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCartVM cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart.ShoppingCart.AppUserId = claim.Value;

            ShoppingCart cartDb = await _unitOfWork.ShoppingCart.GetById(c => c.AppUserId == claim.Value &&
                                                                         c.ProductId == cart.ShoppingCart.ProductId);
            if (cartDb == null)
            {
                await _unitOfWork.ShoppingCart.Add(cart.ShoppingCart);
            }
            else
            {
                cartDb.Quantity += cart.ShoppingCart.Quantity;
                _unitOfWork.ShoppingCart.Update(cartDb);
            }

            await _unitOfWork.Save();
            TempData[DS.Success] = "Producto agregado al carro de compras";

            //Add sesion value
            var cartList = await _unitOfWork.ShoppingCart.GetAll(c => c.AppUserId == claim.Value);
            var numberProduct = cartList.Count();

            HttpContext.Session.SetInt32(DS.ssShoppingCart, numberProduct);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}