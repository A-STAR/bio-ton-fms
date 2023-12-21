using BioTonFMS.Telematica.Dtos.MessagesView;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class MessagesViewMessagesRequestValidator : AbstractValidator<MessagesViewMessagesRequest>
    {
        public MessagesViewMessagesRequestValidator()
        {
            RuleFor(x => x.PageNum)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Номер страницы для постраничного вывода должен быть больше 0");

            RuleFor(x => x.PageSize)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Размер страницы для постраничного вывода должен быть больше 0");
        }
    }
}
