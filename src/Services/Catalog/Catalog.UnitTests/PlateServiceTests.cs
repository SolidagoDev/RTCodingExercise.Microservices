using Catalog.API.Data;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlateServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PlateService _service;

        public PlateServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            var logger = new Mock<ILogger<PlateService>>();
            var publishEndpoint = new Mock<MassTransit.IPublishEndpoint>();
            _service = new PlateService(_context, logger.Object, publishEndpoint.Object);
            Seed();
        }

        private void Seed()
        {
            _context.Plates.Add(new Plate { Id = Guid.NewGuid(), Registration = "TEST1", PurchasePrice = 100, SalePrice = 200, Letters = "TST", Numbers = 1, Status = PlateStatus.ForSale });
            _context.Plates.Add(new Plate { Id = Guid.NewGuid(), Registration = "TEST2", PurchasePrice = 150, SalePrice = 300, Letters = "ABC", Numbers = 2, Status = PlateStatus.ForSale });
            _context.SaveChanges();
        }

        [Fact]
        public async Task Can_Reserve_Plate()
        {
            var plate = _context.Plates.First();
            var result = await _service.ReservePlateAsync(plate.Id, "user1");
            Assert.NotNull(result);
            Assert.Equal(PlateStatus.Reserved, result.Status);
            Assert.Equal("user1", result.ReservedBy);
        }

        [Fact]
        public async Task Can_Sell_Plate()
        {
            var plate = _context.Plates.First();
            await _service.ReservePlateAsync(plate.Id, "user1");
            var result = await _service.SellPlateAsync(plate.Id, "buyer1");
            Assert.NotNull(result);
            Assert.Equal(PlateStatus.Sold, result.Status);
            Assert.Equal("buyer1", result.SoldTo);
        }

        [Fact]
        public async Task Can_Apply_Discount()
        {
            var plate = _context.Plates.First();
            var discounted = await _service.ApplyDiscountAsync(plate.Id, "DISCOUNT");
            Assert.Equal(plate.SalePrice - 25, discounted);
        }

        [Fact]
        public async Task Discount_Below_90_Percent_Not_Allowed()
        {
            var plate = _context.Plates.First();
            plate.SalePrice = 20;
            _context.SaveChanges();
            var discounted = await _service.ApplyDiscountAsync(plate.Id, "DISCOUNT");
            Assert.Null(discounted);
        }

        [Fact]
        public async Task Can_Filter_By_Name()
        {
            var results = await _service.GetPlatesAsync(0, 10, "TST");
            Assert.Single(results);
        }

        [Fact]
        public async Task Can_Unreserve_Plate()
        {
            var plate = _context.Plates.First();
            await _service.ReservePlateAsync(plate.Id, "user1");
            var result = await _service.UnreservePlateAsync(plate.Id, "user1");
            Assert.NotNull(result);
            Assert.Equal(PlateStatus.ForSale, result.Status);
        }
    }
}
