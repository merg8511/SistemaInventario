using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _webUrl;

        [BindProperty]
        private ShoppingCartVM cart { get; set; }

        public CartController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _webUrl = configuration.GetValue<string>("DomainUrl:WEB_URL");
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

        public async Task<IActionResult> Proceed()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart = new ShoppingCartVM
            {
                Order = new Order(),
                ShoppingCarts = await _unitOfWork.ShoppingCart.GetAll(
                    s => s.AppUserId == claim.Value, includeProperties: "Product"),
                Company = await _unitOfWork.Company.GetById()
            };

            cart.Order.TotalOrder = 0;
            cart.Order.AppUser = await _unitOfWork.AppUser.GetById(u => u.Id == claim.Value);

            foreach (var item in cart.ShoppingCarts)
            {
                item.Price = item.Product.Price;
                cart.Order.TotalOrder += (item.Price * item.Quantity);
            }

            cart.Order.ClientName = cart.Order.AppUser.Names + " " + cart.Order.AppUser.LastName;
            cart.Order.Phone = cart.Order.AppUser.PhoneNumber;
            cart.Order.Address = cart.Order.AppUser.Address;
            cart.Order.Country = cart.Order.AppUser.Country;
            cart.Order.City = cart.Order.AppUser.City;

            foreach (var item in cart.ShoppingCarts)
            {
                // get product stock
                var product = await _unitOfWork.WarehouseProduct.GetById(p => p.ProductId == item.ProductId
                && p.WarehouseId == cart.Company.WarehouseSaleId);

                if (item.Quantity > product.Quantity)
                {
                    TempData[DS.Error] = $"La cantidad de producto {item.Product.Description} excede al stock actual {product.Quantity}";
                    return RedirectToAction("Index");
                }
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceed(ShoppingCartVM cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cart.ShoppingCarts = await _unitOfWork.ShoppingCart.GetAll(s => s.AppUserId == claim.Value,
                includeProperties: "Product");

            cart.Company = await _unitOfWork.Company.GetById();

            cart.Order.TotalOrder = 0;
            cart.Order.AppUserId = claim.Value;
            cart.Order.OrderDate = DateTime.Now;

            foreach (var item in cart.ShoppingCarts)
            {
                item.Price = item.Product.Price;
                cart.Order.TotalOrder += (item.Price * item.Quantity);
            }

            foreach (var item in cart.ShoppingCarts)
            {
                // get product stock
                var product = await _unitOfWork.WarehouseProduct.GetById(p => p.ProductId == item.ProductId
                && p.WarehouseId == cart.Company.WarehouseSaleId);

                if (item.Quantity > product.Quantity)
                {
                    TempData[DS.Error] = $"La cantidad de producto {item.Product.Description} excede al stock actual {product.Quantity}";
                    return RedirectToAction("Index");
                }
            }

            cart.Order.OrderState = DS.PendingState;
            cart.Order.PaymentState = DS.PaymentPendingState;

            await _unitOfWork.Order.Add(cart.Order);
            await _unitOfWork.Save();

            //Save details
            foreach (var item in cart.ShoppingCarts)
            {
                var detail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = cart.Order.Id,
                    Price = item.Price,
                    Quantity = item.Quantity
                };

                await _unitOfWork.OrderDetail.Add(detail);
                await _unitOfWork.Save();
            }

            //stripe
            var user = await _unitOfWork.AppUser.GetById(u => u.Id == claim.Value);
            var options = new SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"Inventory/Cart/OrderConfirmation?id={cart.Order.Id}",
                CancelUrl = _webUrl + $"Inventory/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = user.Email
            };

            foreach (var item in cart.ShoppingCarts)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Description
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Order.UpdateStripePaymentId(cart.Order.Id, session.Id, session.PaymentIntentId);
            await _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _unitOfWork.Order.GetById(o => o.Id == id, includeProperties: "AppUser");
            var service = new SessionService();
            Session session = service.Get(order.SessionId);

            var shoppingCart = await _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == order.AppUserId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.Order.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.Order.UpdateState(id, DS.ApprovedState, DS.PaymentApprovedState);

                //disminuir stock
                var company = await _unitOfWork.Company.GetById();
                foreach (var item in shoppingCart)
                {
                    var warehouseProduct = new WarehouseProduct();
                    warehouseProduct = await _unitOfWork.WarehouseProduct.GetById(w => w.ProductId == item.ProductId
                    && w.WarehouseId == company.WarehouseSaleId);

                    await _unitOfWork.KardexStock.KardexRecord(warehouseProduct.Id, "Output", "Venta - Orden# " + id,
                        warehouseProduct.Quantity, item.Quantity, order.AppUserId);

                    warehouseProduct.Quantity -= item.Quantity;
                    await _unitOfWork.Save();
                }
            }

            //borrar carro de compras y session
            List<ShoppingCart> shoppingCartList = shoppingCart.ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartList);
            await _unitOfWork.Save();

            HttpContext.Session.SetInt32(DS.ssShoppingCart, 0);

            return View(id);
        }
    }
}
