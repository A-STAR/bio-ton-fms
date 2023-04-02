using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class SensorTypeRepositoryMock
{
    public const int ExistentSensorTypeId = 3;
    public const int NonExistentSensorTypeId = -1;

    public const int SensorTypeWithBooleanDataTypeId = 1;
    public const int SensorTypeWithSecondUnitId = 2;

    private static List<SensorType> GetSensorTypes() =>
        new()
        {
            new SensorType(1, 1, "SensorType 1", "Sensor type having it's data type set to Boolean. It should constrain data types of sensors of this type.", SensorDataTypeEnum.Boolean, null),
            new SensorType(2, 1, "SensorType 2", "Sensor type having it's measurement unit set to Second. It should constrain units of sensors of this type.", null, 2),
            new SensorType(3, 1, "SensorType 3", "Sensor type with empty unit and data type. It should constrain nothing.", null, null),
        };

    public static ISensorTypeRepository GetStub()
    {
        var repo = new Mock<ISensorTypeRepository>();
        repo.Setup(x => x.GetSensorTypes())
            .Returns(GetSensorTypes);
        repo.Setup(x => x[It.IsAny<int>()])
            .Returns((int i) => GetSensorTypes().FirstOrDefault(x => x.Id == i));
        return repo.Object;
    }
}
