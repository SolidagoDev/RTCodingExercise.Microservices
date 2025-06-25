using RTCodingExercise.Microservices.Models;
using System.Diagnostics;
using WebMVC.Services;
using WebMVC.ViewModels;
using WebMVC.ViewModels.CatalogViewModels;
using System.Text.Json;

namespace RTCodingExercise.Microservices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICatalogService _catalogSvc;

        public HomeController(ILogger<HomeController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogSvc = catalogService;
        }

        public async Task<IActionResult> Index(int? page, string? filter, string? orderBy, int? pageSize)
        {
            var itemsPage = pageSize ?? 10;
            var catalog = await _catalogSvc.GetPlatesAsync(page ?? 0, itemsPage, filter, orderBy);
            var vm = new IndexViewModel()
            {
                Plates = catalog.Data,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = itemsPage,
                    TotalItems = catalog.Count,
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPage))
                }
            };
            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
            // Get revenue and profit margin
            using var client = new HttpClient();
            string? revenueResp = null;
            string? marginResp = null;
            try {
                revenueResp = await client.GetStringAsync("http://catalog-api/api/v1/plates/total-revenue");
            } catch { }
            try {
                marginResp = await client.GetStringAsync("http://catalog-api/api/v1/plates/average-profit-margin");
            } catch { }
            decimal totalRevenue = 0;
            decimal avgProfitMargin = 0;
            if (!string.IsNullOrEmpty(revenueResp))
            {
                try {
                    var revenueObj = JsonDocument.Parse(revenueResp);
                    if (revenueObj.RootElement.TryGetProperty("TotalRevenue", out var rev))
                        totalRevenue = rev.GetDecimal();
                } catch { }
            }
            if (!string.IsNullOrEmpty(marginResp))
            {
                try {
                    var marginObj = JsonDocument.Parse(marginResp);
                    if (marginObj.RootElement.TryGetProperty("AverageProfitMargin", out var mar))
                        avgProfitMargin = mar.GetDecimal();
                } catch { }
            }
            ViewBag.TotalRevenue = totalRevenue.ToString("C");
            ViewBag.AvgProfitMargin = avgProfitMargin.ToString("F2");
            ViewBag.Filter = filter;
            ViewBag.OrderBy = orderBy;
            return View(vm);
        }

        [HttpGet]
        public IActionResult AddPlate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPlateSubmit(Plate plate)
        {
            await _catalogSvc.InsertPlateAsync(plate);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReservePlate(Guid id, string reservedBy)
        {
            // Call API to reserve plate
            using var client = new HttpClient();
            var uri = $"http://catalog-api/api/v1/plates/reserve/{id}?reservedBy={reservedBy}";
            await client.PostAsync(uri, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnreservePlate(Guid id, string unreservedBy)
        {
            // Call API to unreserve plate
            using var client = new HttpClient();
            var uri = $"http://catalog-api/api/v1/plates/unreserve/{id}?unreservedBy={unreservedBy}";
            await client.PostAsync(uri, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SellPlate(Guid id, string soldTo)
        {
            // Call API to sell plate
            using var client = new HttpClient();
            var uri = $"http://catalog-api/api/v1/plates/sell/{id}?soldTo={soldTo}";
            await client.PostAsync(uri, null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(Guid plateId, string promoCode)
        {
            using var client = new HttpClient();
            var uri = "http://catalog-api/api/v1/plates/apply-discount";
            var request = new { PlateId = plateId, PromoCode = promoCode };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                TempData["DiscountResult"] = result;
            }
            else
            {
                TempData["DiscountResult"] = "Discount code is not applicable or plate not found.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AuditLogs(Guid id)
        {
            using var client = new HttpClient();
            var uri = $"http://catalog-api/api/v1/plates/audit/{id}";
            var response = await client.GetStringAsync(uri);
            ViewBag.AuditLogs = response;
            return View();
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