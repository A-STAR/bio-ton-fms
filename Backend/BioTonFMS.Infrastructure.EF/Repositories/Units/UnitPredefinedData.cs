using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.Units;

public static class UnitPredefinedData
{
    public static readonly IEnumerable<Unit> Units = new[]
    {
        new Unit(1, "Безразмерная величина", ""),
        new Unit(2, "Километры", "км"),
        new Unit(3, "Вольты", "В"),
        new Unit(4, "Тонны", "т"),
        new Unit(5, "g", "g"),
        new Unit(6, "Градусы цельсия", "C°"),
        new Unit(7, "Обороты в минуту", "об/мин"),
        new Unit(8, "Часы", "ч"),
        new Unit(9, "Литры", "л"),
    };
}
