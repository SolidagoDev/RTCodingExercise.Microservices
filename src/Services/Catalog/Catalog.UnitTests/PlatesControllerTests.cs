using Catalog.API.Controllers;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlatesControllerTests
    {
        private readonly PlatesController _controller;
        private readonly Mock<IPlateService> _serviceMock;
        private readonly Mock<ILogger<PlatesController>> _loggerMock;

        public PlatesControllerTests()
        {
            _serviceMock = new Mock<IPlateService>();
            _loggerMock = new Mock<ILogger<PlatesController>>();
            _controller = new PlatesController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task PlatesAsync_Returns_Ok()
        {
            _serviceMock.Setup(s => s.GetPlatesAsync(0, 10, null, null, false)).ReturnsAsync(new List<Plate>());
            var result = await _controller.PlatesAsync();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReservePlate_Returns_Ok_When_Success()
        {
            var plate = new Plate { Id = Guid.NewGuid(), Status = PlateStatus.ForSale };
            _serviceMock.Setup(s => s.ReservePlateAsync(plate.Id, "user")).ReturnsAsync(plate);
            var result = await _controller.ReservePlate(plate.Id, "user");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReservePlate_Returns_NotFound_When_Fail()
        {
            _serviceMock.Setup(s => s.ReservePlateAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync((Plate)null);
            var result = await _controller.ReservePlate(Guid.NewGuid(), "user");
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
