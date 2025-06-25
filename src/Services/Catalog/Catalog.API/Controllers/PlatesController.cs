using Catalog.API.ViewModels;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlatesController : ControllerBase
    {
        private readonly ILogger<PlatesController> _logger;
        private readonly IPlateService _plateService;

        public PlatesController(ILogger<PlatesController> logger, IPlateService plateService)
        {
            _logger = logger;
            _plateService = plateService;
        }

        // GET api/v1/[controller]/plates[?pageSize=3&pageIndex=10&filter=abc&orderBy=price&onlyForSale=true]
        [HttpGet]
        [Route("plates")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Plate>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> PlatesAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, [FromQuery] string? filter = null, [FromQuery] string? orderBy = null, [FromQuery] bool onlyForSale = false)
        {
            var query = _plateService.GetQueryablePlates(filter, onlyForSale); // new helper method
            var totalItems = await query.CountAsync();
            var itemsOnPage = await _plateService.GetPlatesAsync(pageIndex, pageSize, filter, orderBy, onlyForSale);
            var dtos = itemsOnPage.Select(p => new PlateDto
            {
                Id = p.Id,
                Registration = p.Registration,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice,
                Status = p.Status,
                Letters = p.Letters,
                Numbers = p.Numbers,
                ReservedAt = p.ReservedAt,
                SoldAt = p.SoldAt,
                ReservedBy = p.ReservedBy,
                SoldTo = p.SoldTo
            }).ToList();
            var model = new PaginatedItemsViewModel<PlateDto>(pageIndex, pageSize, totalItems, dtos);
            return Ok(model);
        }

        [HttpPost]
        [Route("insertplate")]
        public async Task<ActionResult> InsertPlate(Plate plate)
        {
            var result = await _plateService.AddPlateAsync(plate);
            return Ok(result);
        }

        [HttpPost]
        [Route("reserve/{id}")]
        public async Task<ActionResult> ReservePlate(Guid id, [FromQuery] string reservedBy)
        {
            var result = await _plateService.ReservePlateAsync(id, reservedBy);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Route("sell/{id}")]
        public async Task<ActionResult> SellPlate(Guid id, [FromQuery] string soldTo)
        {
            var result = await _plateService.SellPlateAsync(id, soldTo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Route("apply-discount")]
        public async Task<ActionResult> ApplyDiscount([FromBody] DiscountRequestDto request)
        {
            var discountedPrice = await _plateService.ApplyDiscountAsync(request.PlateId, request.PromoCode);
            if (discountedPrice == null)
                return BadRequest("Discount code is not applicable or plate not found.");
            return Ok(new { PlateId = request.PlateId, DiscountedPrice = discountedPrice });
        }

        [HttpGet]
        [Route("total-revenue")]
        public async Task<ActionResult> GetTotalRevenue()
        {
            var revenue = await _plateService.GetTotalRevenueAsync();
            return Ok(new { TotalRevenue = revenue });
        }

        [HttpGet]
        [Route("average-profit-margin")]
        public async Task<ActionResult> GetAverageProfitMargin()
        {
            var margin = await _plateService.GetAverageProfitMarginAsync();
            return Ok(new { AverageProfitMargin = margin });
        }

        [HttpGet]
        [Route("audit/{id}")]
        public async Task<ActionResult> GetAuditLogs(Guid id)
        {
            var logs = await _plateService.GetAuditLogsAsync(id);
            return Ok(logs);
        }

        [HttpPost]
        [Route("unreserve/{id}")]
        public async Task<ActionResult> UnreservePlate(Guid id, [FromQuery] string unreservedBy)
        {
            var result = await _plateService.UnreservePlateAsync(id, unreservedBy);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
