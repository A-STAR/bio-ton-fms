using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Repositories.SensorTypes;
using BioTonFMS.Infrastructure.EF.Repositories.Trackers;
using BioTonFMS.Infrastructure.EF.Repositories.Units;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class SensorValidator : AbstractValidator<Sensor>
    {
        public SensorValidator(ITrackerRepository trackerRepository, IUnitRepository unitRepository, ISensorTypeRepository sensorTypeRepository)
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.Description).Length(0, 500);
            RuleFor(x => x.Formula).Length(0, 500);
            RuleFor(x => x.TrackerId).Must(trackerId => trackerRepository[trackerId] is not null)
                .WithMessage(x => $"Трекер с идентификатором {x.TrackerId} не существует!");
            RuleFor(x => x.UnitId).Must(unitId => unitRepository[unitId] is not null)
                .WithMessage(x => $"Единица измерения с идентификатором {x.UnitId} не существует!");
            RuleFor(x => x.SensorTypeId).Must(sensorTypeId => sensorTypeRepository[sensorTypeId] is not null)
                .WithMessage(x => $"Тип датчиков с идентификатором {x.SensorTypeId} не существует!");
            RuleFor(x => x.FuelUse).GreaterThan(0);
        }
    }
}
