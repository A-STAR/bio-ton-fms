using BioTonFMS.Telematica.Dtos.Tracker;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class UpdateTrackerDtoValidator : AbstractValidator<UpdateTrackerDto>
    {
        public UpdateTrackerDtoValidator()
        {
            RuleFor(x => x.ExternalId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.SimNumber).NotEmpty().Length(1, 12);
            When(x => x.Description != string.Empty, () => RuleFor(x => x.Description).Length(1, 500));
        }
    }
}
