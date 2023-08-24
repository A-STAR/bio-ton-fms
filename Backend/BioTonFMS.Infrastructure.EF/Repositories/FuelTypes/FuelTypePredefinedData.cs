using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.FuelTypes;

public static class FuelTypePredefinedData
{
    public static readonly IEnumerable<FuelType> FuelTypes = new[]
    {
        new FuelType()
        {
            Id = 1,
            Name = "Бензин АИ-95"
        },
        new FuelType()
        {
            Id = 2,
            Name = "Дизельное топливо"
        }
    };
}
