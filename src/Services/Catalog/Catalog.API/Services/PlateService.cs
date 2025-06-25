using MassTransit;
using BuildingBlocks.IntegrationEvents;

namespace Catalog.API.Services
{
    public interface IPlateService
    {
        Task<IEnumerable<Plate>> GetPlatesAsync(int pageIndex, int pageSize, string? filter = null, string? orderBy = null, bool onlyForSale = false);
        Task<Plate?> GetPlateByIdAsync(Guid id);
        Task<Plate?> ReservePlateAsync(Guid id, string reservedBy);
        Task<Plate?> SellPlateAsync(Guid id, string soldTo);
        Task<Plate?> AddPlateAsync(Plate plate);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetAverageProfitMarginAsync();
        Task<decimal?> ApplyDiscountAsync(Guid plateId, string promoCode);
        Task<IEnumerable<Data.PlateAuditLog>> GetAuditLogsAsync(Guid plateId);
        Task<Plate?> UnreservePlateAsync(Guid id, string unreservedBy);
        IQueryable<Plate> GetQueryablePlates(string? filter = null, bool onlyForSale = false);
    }

    public class PlateService : IPlateService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlateService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public PlateService(ApplicationDbContext context, ILogger<PlateService> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<IEnumerable<Plate>> GetPlatesAsync(int pageIndex, int pageSize, string? filter = null, string? orderBy = null, bool onlyForSale = false)
        {
            var query = _context.Plates.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                // Advanced: If filter is a name, match plates that look like the name (e.g., Danny -> DA12 NNY)
                var filterUpper = filter.ToUpperInvariant();
                query = query.Where(p =>
                    (p.Registration != null && p.Registration.Replace(" ", "").ToUpper().Contains(filterUpper.Replace(" ", ""))) ||
                    (p.Letters != null && p.Letters.ToUpper().Contains(filterUpper)) ||
                    p.Numbers.ToString().Contains(filter)
                );
            }
            if (onlyForSale)
            {
                query = query.Where(p => p.Status == PlateStatus.ForSale);
            }
            if (!string.IsNullOrEmpty(orderBy) && orderBy.ToLower() == "price")
            {
                query = query.OrderBy(p => p.SalePrice);
            }
            else
            {
                query = query.OrderBy(p => p.Registration);
            }
            return await query.Skip(pageIndex * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _context.Plates.FindAsync(id);
        }

        public async Task<Plate?> ReservePlateAsync(Guid id, string reservedBy)
        {
            var plate = await _context.Plates.FindAsync(id);
            if (plate == null || plate.Status != PlateStatus.ForSale)
                return null;
            plate.Status = PlateStatus.Reserved;
            plate.ReservedAt = DateTime.UtcNow;
            plate.ReservedBy = reservedBy;
            _context.PlateAuditLogs.Add(new Data.PlateAuditLog
            {
                PlateId = plate.Id,
                Action = "Reserved",
                PerformedBy = reservedBy,
                PerformedAt = plate.ReservedAt.Value,
                Status = PlateStatus.Reserved
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Plate {plate.Registration} reserved by {reservedBy} at {plate.ReservedAt}");
            // Publish event
            await _publishEndpoint.Publish(new PlateReservedIntegrationEvent
            {
                PlateId = plate.Id,
                Registration = plate.Registration ?? string.Empty,
                ReservedBy = reservedBy,
                ReservedAt = plate.ReservedAt.Value,
                Status = (int)plate.Status
            });
            return plate;
        }

        public async Task<Plate?> SellPlateAsync(Guid id, string soldTo)
        {
            var plate = await _context.Plates.FindAsync(id);
            if (plate == null || plate.Status == PlateStatus.Sold)
                return null;
            plate.Status = PlateStatus.Sold;
            plate.SoldAt = DateTime.UtcNow;
            plate.SoldTo = soldTo;
            _context.PlateAuditLogs.Add(new Data.PlateAuditLog
            {
                PlateId = plate.Id,
                Action = "Sold",
                PerformedBy = soldTo,
                PerformedAt = plate.SoldAt.Value,
                Status = PlateStatus.Sold
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Plate {plate.Registration} sold to {soldTo} at {plate.SoldAt}");
            // Publish event
            await _publishEndpoint.Publish(new PlateSoldIntegrationEvent
            {
                PlateId = plate.Id,
                Registration = plate.Registration ?? string.Empty,
                SoldTo = soldTo,
                SoldAt = plate.SoldAt.Value,
                Status = (int)plate.Status
            });
            return plate;
        }

        public async Task<Plate?> AddPlateAsync(Plate plate)
        {
            plate.Status = PlateStatus.ForSale;
            _context.Plates.Add(plate);
            await _context.SaveChangesAsync();
            return plate;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Plates.Where(p => p.Status == PlateStatus.Sold).SumAsync(p => p.SalePrice);
        }

        public async Task<decimal> GetAverageProfitMarginAsync()
        {
            var soldPlates = await _context.Plates.Where(p => p.Status == PlateStatus.Sold).ToListAsync();
            if (!soldPlates.Any()) return 0;
            return soldPlates.Average(p => (p.SalePrice - p.PurchasePrice) / p.SalePrice * 100);
        }

        public async Task<decimal?> ApplyDiscountAsync(Guid plateId, string promoCode)
        {
            var plate = await _context.Plates.FindAsync(plateId);
            if (plate == null || plate.Status != PlateStatus.ForSale)
                return null;
            decimal discountedPrice = plate.SalePrice;
            if (promoCode == "DISCOUNT")
            {
                discountedPrice -= 25;
            }
            else if (promoCode == "PERCENTOFF")
            {
                discountedPrice *= 0.85m;
            }
            // Enforce minimum sale price (90% of original sale price)
            if (discountedPrice < plate.SalePrice * 0.9m)
                return null;
            return discountedPrice;
        }

        public async Task<IEnumerable<Data.PlateAuditLog>> GetAuditLogsAsync(Guid plateId)
        {
            return await _context.PlateAuditLogs
                .Where(log => log.PlateId == plateId)
                .OrderByDescending(log => log.PerformedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Plate?> UnreservePlateAsync(Guid id, string unreservedBy)
        {
            var plate = await _context.Plates.FindAsync(id);
            if (plate == null || plate.Status != PlateStatus.Reserved)
                return null;
            plate.Status = PlateStatus.ForSale;
            plate.ReservedAt = null;
            plate.ReservedBy = null;
            _context.PlateAuditLogs.Add(new Data.PlateAuditLog
            {
                PlateId = plate.Id,
                Action = "Unreserved",
                PerformedBy = unreservedBy,
                PerformedAt = DateTime.UtcNow,
                Status = PlateStatus.ForSale
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Plate {plate.Registration} unreserved by {unreservedBy}");
            return plate;
        }

        public IQueryable<Plate> GetQueryablePlates(string? filter = null, bool onlyForSale = false)
        {
            var query = _context.Plates.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                var filterUpper = filter.ToUpperInvariant();
                query = query.Where(p =>
                    (p.Registration != null && p.Registration.Replace(" ", "").ToUpper().Contains(filterUpper.Replace(" ", ""))) ||
                    (p.Letters != null && p.Letters.ToUpper().Contains(filterUpper)) ||
                    p.Numbers.ToString().Contains(filter)
                );
            }
            if (onlyForSale)
            {
                query = query.Where(p => p.Status == PlateStatus.ForSale);
            }
            return query;
        }
    }
}
