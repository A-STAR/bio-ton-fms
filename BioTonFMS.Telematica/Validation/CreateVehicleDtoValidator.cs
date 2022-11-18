using BioTonFMS.Telematica.Dtos;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.Make).NotEmpty().Length(1, 30);
            RuleFor(x => x.Model).NotEmpty().Length(1, 30);
            RuleFor(x => x.FuelTypeId).NotEmpty();
            RuleFor(x => x.ManufacturingYear).GreaterThan(0).LessThan(DateTime.Now.Year + 1);
            When(x => x.InventoryNumber != string.Empty, () => RuleFor(x => x.InventoryNumber).Length(1, 30));
            When(x => x.SerialNumber != string.Empty, () => RuleFor(x => x.SerialNumber).Length(1, 40));
            When(x => x.RegistrationNumber != string.Empty, 
                () => RuleFor(x => x.RegistrationNumber)
                    .Length(1, 15)
                    .Matches("^[0-9А-Яа-яa-zA-Z]*$")
                    .WithMessage("'Registration Number' имеет неверный формат, поле допускает использование русских и латинских букв и цифр"));
            When(x => x.Description != string.Empty, () => RuleFor(x => x.Description).Length(1, 500));
        }
    }
}
