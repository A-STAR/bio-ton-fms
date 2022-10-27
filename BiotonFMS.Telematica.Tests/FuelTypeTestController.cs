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
    public class FuelTypeTestController
    {
        [Fact]
        public void GetFuelTypes_ShouldReturn200Status()
        {
            var loggerMock = Mock.Of<ILogger<FuelTypeController>>();

            var fuelTypeRepositoryMock = new Mock<IFuelTypeRepository>();

            fuelTypeRepositoryMock.Setup(_ => _.GetFuelTypes()).Returns(FuelTypeMockData.GetFuelTypes());

            MapperConfiguration mapperConfig = new(cfg => cfg.AddProfile(new FuelTypeMappingProfile()));

            IMapper mapper = new Mapper(mapperConfig);

            FuelTypeController fuelTypeController = new(loggerMock, fuelTypeRepositoryMock.Object, mapper);

            var result = (OkObjectResult)fuelTypeController.GetFuelTypes();

            result.StatusCode.Should().Be(200);
        }
    }
}
