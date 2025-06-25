using Catalog.API.Controllers;
using Catalog.API.Data;
using Catalog.API.ViewModels;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class CatalogControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public CatalogControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            using (var dbContext = new ApplicationDbContext(_dbOptions))
            {
                dbContext.AddRange(GetFakePlates());
                dbContext.SaveChanges();
            }
        }

        //[Fact]
        //public async Task Get_Catalog_Plates_Success()
        //{
        //    //Arrange
        //    var pageSize = 4;
        //    var pageIndex = 1;
        //    var expectedItemsInPage = 2;
        //    var expectedTotalItems = 6;
        //    var catalogContext = new ApplicationDbContext(_dbOptions);

        //    //Act
        //    var platesController = new PlatesController(null, catalogContext);
        //    var actionResult = await platesController.PlatesAsync(pageSize, pageIndex);
        //    var okResult = actionResult as OkObjectResult;
        //    var page = (PaginatedItemsViewModel<Plate>)okResult.Value;

        //    //Assert
        //    Assert.Equal(expectedTotalItems, page.Count);
        //    Assert.Equal(pageIndex, page.PageIndex);
        //    Assert.Equal(pageSize, page.PageSize);
        //    Assert.Equal(expectedItemsInPage, page.Data.Count());
        //}

        private List<Plate> GetFakePlates()
        {
            return new List<Plate>()
            {
                new Plate()
                {
                    Id = System.Guid.Parse("0812851E-3EC3-4D12-BAF6-C9F0E6DC2F76"),
                    Registration = "T44GUE",
                    PurchasePrice = 2722.51M,
                    SalePrice = 8995.00M,
                    Numbers = 44,
                    Letters = "TAG"
                },
                new Plate()
                {
                    Id = System.Guid.Parse("DF81D7FC-319B-46A8-AB66-2574B4169C3D"),
                    Registration = "M44BEY",
                    PurchasePrice = 859.10M,
                    SalePrice = 8995.00M,
                    Numbers = 44,
                    Letters = "MAB"
                },
                new Plate()
                {
                    Id = System.Guid.Parse("0E9C83BF-94E2-484A-97CB-A8B06E3410FD"),
                    Registration = "P777PER",
                    PurchasePrice = 1494.08M,
                    SalePrice = 4995.00M,
                    Numbers = 777,
                    Letters = "PYP"
                },
                new Plate()
                {
                    Id = System.Guid.Parse("7C88B586-AABA-400A-8EF2-AF2073FC0CB2"),
                    Registration = "M66VEY",
                    PurchasePrice = 469.26M,
                    SalePrice = 5995.00M,
                    Numbers = 66,
                    Letters = "MCV"
                },
            };
        }
    }
}