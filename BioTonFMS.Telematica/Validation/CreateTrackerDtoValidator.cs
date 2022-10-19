using BioTonFMS.Telematica.Dtos;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class CreateTrackerDtoValidator : AbstractValidator<CreateTrackerDto>
    {
        public CreateTrackerDtoValidator()
        {
            RuleFor(x => x.ExternalId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.SimNumber).NotEmpty().Length(1, 12);
            RuleFor(x => x.Description).Length(1, 500);
        }
    }
}
