using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.SensorGroups;

public static class SensorGroupPredefinedData
{
    public static readonly IEnumerable<SensorGroup> SensorGroups = new[]
    {
        new SensorGroup(1, "Пробег"),
        new SensorGroup(2, "Цифровые"),
        new SensorGroup(3, "Показатели"),
        new SensorGroup(4, "Двигатель"),
        new SensorGroup(5, "Топливо"),
        new SensorGroup(6, "Другие"),
    };
}
