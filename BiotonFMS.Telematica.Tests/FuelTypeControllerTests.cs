using AutoMapper;
using BiotonFMS.Telematica.Tests.Mocks;
using BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;
using BioTonFMS.Telematica.Controllers;
using BioTonFMS.Telematica.Mapping;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BiotonFMS.Telematica.Tests
{
    public class FuelTypeControllerTests
    {
        [Fact]
        public void GetFuelTypes_Always_ShouldReturn200Status()
        {
            // Arrange
            var loggerDummy = Mock.Of<ILogger<FuelTypeController>>();
            var fuelTypeRepositoryStub = new Mock<IFuelTypeRepository>();
            fuelTypeRepositoryStub.Setup(_ => _.GetFuelTypes()).Returns(FuelTypeFakeData.GetFuelTypes());

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new FuelTypeMappingProfile()));
            IMapper mapper = new Mapper(mapperConfig);

            var fuelTypeController = new FuelTypeController(loggerDummy, fuelTypeRepositoryStub.Object, mapper);

            // Act
            var result = (OkObjectResult)fuelTypeController.GetFuelTypes();

            // Assert
            result.StatusCode.Should().Be(200);
        }
    }
}
