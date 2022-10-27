using BioTonFMS.Domain;

namespace BiotonFMS.Telematica.Tests.Mocks
{
    public class FuelTypeMockData
    {
        public static IEnumerable<FuelType> GetFuelTypes()
        {
            return new List<FuelType>{
                 new FuelType
                 {
                     Id = 1,
                     Name = "Бензин"
                 },
                 new FuelType
                 {
                     Id = 1,
                     Name = "Дизельное топливо"
                 }
            };
        }
    }
}
