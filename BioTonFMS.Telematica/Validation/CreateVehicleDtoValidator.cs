using BioTonFMS.Telematica.Dtos;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.VehicleGroupId).NotEmpty();
            RuleFor(x => x.Make).NotEmpty().Length(1, 30);
            RuleFor(x => x.Model).NotEmpty().Length(1, 30);
            RuleFor(x => x.FuelTypeId).NotEmpty();
            When(x => x.InventoryNumber != string.Empty, () => RuleFor(x => x.InventoryNumber).Length(1, 30));
            When(x => x.SerialNumber != string.Empty, () => RuleFor(x => x.RegistrationNumber).Length(1, 40));
            When(x => x.RegistrationNumber != string.Empty, () => RuleFor(x => x.RegistrationNumber).Length(1, 15));
            When(x => x.Description != string.Empty, () => RuleFor(x => x.Description).Length(1, 500));
        }
    }
}
