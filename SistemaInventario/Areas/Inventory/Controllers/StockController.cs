using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using SistemaInventario.Models.ViewModels;
using SistemaInventario.Utilities;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Inventory)]
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public StockVM stockVM { get; set; }

        public StockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewStock()
        {
            stockVM = new()
            {
                Stock = new Stock(),
                Warehouses = _unitOfWork.Stock.GetAllDropdownList("Warehouse"),
            };

            stockVM.Stock.State = false;
            //Obtener el Id del usuario desde la sesíon
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            stockVM.Stock.AppUserId = claim.Value;
            stockVM.Stock.InitDate = DateTime.Now;
            stockVM.Stock.FinalDate = DateTime.Now;

            return View(stockVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewStock(StockVM stockVM)
        {
            if (ModelState.IsValid)
            {
                stockVM.Stock.InitDate = DateTime.Now;
                stockVM.Stock.FinalDate = DateTime.Now;
                await _unitOfWork.Stock.Add(stockVM.Stock);
                await _unitOfWork.Save();

                return RedirectToAction("StockDetail", new { id = stockVM.Stock.Id });
            }

            stockVM.Warehouses = _unitOfWork.Stock.GetAllDropdownList("Warehouse");
            return View(stockVM);
        }

        public async Task<IActionResult> StockDetail(int id)
        {
            stockVM = new StockVM();
            stockVM.Stock = await _unitOfWork.Stock.GetById(x => x.Id == id, includeProperties: "Warehouse");
            stockVM.Details = await _unitOfWork.StockDetail.GetAll(d => d.StockId == id,
                includeProperties: "Product,Product.Brand");

            return View(stockVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> StockDetail(int stockId, int productId, int quantity)
        {
            stockVM = new StockVM();
            stockVM.Stock = await _unitOfWork.Stock.GetById(x => x.Id == stockId);

            var warehouseProduct = await _unitOfWork.WarehouseProduct.GetById(w => w.ProductId == productId &&
            w.WarehouseId == stockVM.Stock.WarehouseId);

            var detail = await _unitOfWork.StockDetail.GetById(d => d.StockId == stockId &&
            d.ProductId == productId);

            if (detail == null)
            {
                stockVM.StockDetail = new StockDetail();
                stockVM.StockDetail.ProductId = productId;
                stockVM.StockDetail.StockId = stockId;
                if (warehouseProduct != null)
                {
                    stockVM.StockDetail.LastStock = warehouseProduct.Quantity;
                }
                else
                {
                    stockVM.StockDetail.LastStock = 0;
                }
                stockVM.StockDetail.Quantity = quantity;
                await _unitOfWork.StockDetail.Add(stockVM.StockDetail);
                await _unitOfWork.Save();
            }
            else
            {
                detail.Quantity += quantity;
                await _unitOfWork.Save();
            }
            return RedirectToAction("StockDetail", new { id = stockId });
        }

        public async Task<IActionResult> Plus(int id) //recibe el id del detalle
        {
            stockVM = new();

            var detail = await _unitOfWork.StockDetail.Get(id);
            stockVM.Stock = await _unitOfWork.Stock.Get(detail.StockId);

            detail.Quantity += 1;
            await _unitOfWork.Save();

            return RedirectToAction("StockDetail", new { id = stockVM.Stock.Id });
        }

        public async Task<IActionResult> Less(int id) //recibe el id del detalle
        {
            stockVM = new();

            var detail = await _unitOfWork.StockDetail.Get(id);
            stockVM.Stock = await _unitOfWork.Stock.Get(detail.StockId);

            if (detail.Quantity == 1)
            {
                _unitOfWork.StockDetail.Remove(detail);
                await _unitOfWork.Save();
            }
            else
            {
                detail.Quantity -= 1;
                await _unitOfWork.Save();
            }

            return RedirectToAction("StockDetail", new { id = stockVM.Stock.Id });
        }

        public async Task<IActionResult> GetStock(int id)
        {
            var stock = await _unitOfWork.Stock.Get(id);
            var details = await _unitOfWork.StockDetail.GetAll(d => d.StockId == id);

            //Get user logged
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var detail in details)
            {
                var warehouseProduct = new WarehouseProduct();
                warehouseProduct = await _unitOfWork.WarehouseProduct.GetById(b => b.ProductId == detail.ProductId &&
                b.WarehouseId == stock.WarehouseId);

                if (warehouseProduct != null) //Registro de stock existe, hay que actualizar cantidades
                {
                    await _unitOfWork.KardexStock.KardexRecord(warehouseProduct.Id, "Input", "Registro de inventario",
                        warehouseProduct.Quantity, detail.Quantity, claim.Value);

                    warehouseProduct.Quantity += detail.Quantity;
                    await _unitOfWork.Save();
                }
                else //El registro de stock no existe, hay que crearlo
                {
                    warehouseProduct = new();
                    warehouseProduct.WarehouseId = stock.WarehouseId;
                    warehouseProduct.ProductId = detail.ProductId;
                    warehouseProduct.Quantity = detail.Quantity;
                    await _unitOfWork.WarehouseProduct.Add(warehouseProduct);
                    await _unitOfWork.Save();
                    await _unitOfWork.KardexStock.KardexRecord(warehouseProduct.Id, "Input",
                        "Registro de inventario inicial", 0, detail.Quantity, claim.Value);
                }
            }

            //Actualizar cabecera del inventario
            stock.State = true;
            stock.FinalDate = DateTime.Now;
            await _unitOfWork.Save();
            TempData[DS.Success] = "Stock generado con éxito";

            return RedirectToAction("Index");
        }

        public IActionResult KardexProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProduct(string dateInitId, string dateFinalId, int productId)
        {
            return RedirectToAction("KardexProductResult", new { dateInitId, dateFinalId, productId });
        }

        public async Task<IActionResult> KardexProductResult(string dateInitId, string dateFinalId, int productId)
        {
            KardexStockVM kardexStockVM = new KardexStockVM();
            kardexStockVM.Product = new Product();

            kardexStockVM.Product = await _unitOfWork.Product.Get(productId);

            kardexStockVM.InitDate = DateTime.Parse(dateInitId);
            kardexStockVM.FinalDate = DateTime.Parse(dateFinalId).AddHours(23).AddMinutes(59);

            kardexStockVM.KardexStocks = await _unitOfWork.KardexStock.GetAll(
                k => k.WarehouseProduct.ProductId == productId &&
                (k.RecordDate >= kardexStockVM.InitDate &&
                k.RecordDate <= kardexStockVM.FinalDate),
                includeProperties: "WarehouseProduct,WarehouseProduct.Product,WarehouseProduct.Warehouse",
                orderBy: o => o.OrderBy(o => o.RecordDate)
                );

            return View(kardexStockVM);
        }

        public async Task<IActionResult> PrintReport(string dateInit, string dateFinal, int productId)
        {
            KardexStockVM kardexStockVM = new KardexStockVM();
            kardexStockVM.Product = new Product();

            kardexStockVM.Product = await _unitOfWork.Product.Get(productId);

            kardexStockVM.InitDate = DateTime.Parse(dateInit);
            kardexStockVM.FinalDate = DateTime.Parse(dateFinal);

            kardexStockVM.KardexStocks = await _unitOfWork.KardexStock.GetAll(
                k => k.WarehouseProduct.ProductId == productId &&
                (k.RecordDate >= kardexStockVM.InitDate &&
                k.RecordDate <= kardexStockVM.FinalDate),
                includeProperties: "WarehouseProduct,WarehouseProduct.Product,WarehouseProduct.Warehouse",
                orderBy: o => o.OrderBy(o => o.RecordDate)
                );

            return new ViewAsPdf("PrintReport", kardexStockVM)
            {
                FileName = "KardexProducto.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            };
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitOfWork.WarehouseProduct.GetAll(includeProperties: "Warehouse,Product");
            return Json(new { data = all });
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var products = await _unitOfWork.Product.GetAll(p => p.pState == true);
                var data = products.Where(x => x.SerieNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
                return Ok(data);
            }

            return Ok();

        }

        #endregion
    }
}
