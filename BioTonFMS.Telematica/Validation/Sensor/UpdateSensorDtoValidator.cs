using BioTonFMS.Telematica.Dtos;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class UpdateSensorDtoValidator : AbstractValidator<UpdateSensorDto>
    {
        public UpdateSensorDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            When(x => x.Description != string.Empty, () => RuleFor(x => x.Description).Length(1, 500));
        }
    }
}
