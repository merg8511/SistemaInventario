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
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailVM orderDetailVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            orderDetailVM = new OrderDetailVM()
            {
                Order = await _unitOfWork.Order.GetById(o => o.Id == id, includeProperties: "AppUser"),
                Details = await _unitOfWork.OrderDetail.GetAll(d => d.OrderId == id, includeProperties: "Product"),
            };

            return View(orderDetailVM);
        }

        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> Proceed(int id)
        {
            var order = await _unitOfWork.Order.GetById(o => o.Id == id);
            order.OrderState = DS.InProcessState;
            await _unitOfWork.Save();
            TempData[DS.Success] = "Order cambiada a estado en proceso";

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = DS.Role_Admin)]
        public async Task<IActionResult> SendOrder(OrderDetailVM orderDetailVM)
        {
            var order = await _unitOfWork.Order.GetById(o => o.Id == orderDetailVM.Order.Id);
            order.OrderState = DS.SentState;
            order.Carrier = orderDetailVM.Order.Carrier;
            order.ShippingNumber = orderDetailVM.Order.ShippingNumber;
            order.ShippingDate = DateTime.Now;

            await _unitOfWork.Save();
            TempData[DS.Success] = "Order cambiada a estado en enviado";

            return RedirectToAction("Details", new { id = orderDetailVM.Order.Id });
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetOrders(string state)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Order> orders;

            if (User.IsInRole(DS.Role_Admin))
            {
                orders = await _unitOfWork.Order.GetAll(includeProperties: "AppUser");
            }
            else
            {
                orders = await _unitOfWork.Order.GetAll(o => o.AppUserId == claim.Value, includeProperties: "AppUser");
            }

            switch (state)
            {
                case "approved":
                    orders = orders.Where(o => o.OrderState == DS.ApprovedState);
                    break;
                case "completed":
                    orders = orders.Where(o => o.OrderState == DS.SentState);
                    break;
                default:
                    break;
            }

            return Json(new { data = orders });
        }
        #endregion
    }
}
