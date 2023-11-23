using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        private ShoppingCartVM cart { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart = new ShoppingCartVM();
            cart.Order = new Order();
            cart.ShoppingCarts = await _unitOfWork.ShoppingCart.GetAll(s => s.AppUserId == claim.Value,
                includeProperties: "Product");

            cart.Order.TotalOrder = 0;
            cart.Order.AppUserId = claim.Value;

            foreach (var item in cart.ShoppingCarts)
            {
                item.Price = item.Product.Price; //Siempre mostrar el precio actual del producto
                cart.Order.TotalOrder += (item.Price * item.Quantity);
            }

            return View(cart);
        }

        public async Task<IActionResult> More(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetById(c => c.Id == cartId);
            cart.Quantity += 1;

            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Less(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetById(c => c.Id == cartId);

            if (cart.Quantity == 1)
            {
                //Remover el registro, actualizar la sesión
                var cartList = await _unitOfWork.ShoppingCart.GetAll(c => c.AppUserId == cart.AppUserId);

                var numberProduct = cartList.Count();
                _unitOfWork.ShoppingCart.Remove(cart);
                await _unitOfWork.Save();
                HttpContext.Session.SetInt32(DS.ssShoppingCart, numberProduct - 1);
            }
            else
            {
                cart.Quantity -= 1;
                await _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetById(c => c.Id == cartId);

            //Remover el registro, actualizar la sesión
            var cartList = await _unitOfWork.ShoppingCart.GetAll(c => c.AppUserId == cart.AppUserId);

            var numberProduct = cartList.Count();
            _unitOfWork.ShoppingCart.Remove(cart);
            await _unitOfWork.Save();
            HttpContext.Session.SetInt32(DS.ssShoppingCart, numberProduct - 1);

            return RedirectToAction("Index");
        }
    }
}
