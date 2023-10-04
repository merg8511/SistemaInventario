using Microsoft.AspNetCore.Mvc;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.Specifications;
using SistemaInventario.Models.ViewModels;
using System.Diagnostics;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int pageNumber = 1, string search = "", string currentSearch = "")
        {
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