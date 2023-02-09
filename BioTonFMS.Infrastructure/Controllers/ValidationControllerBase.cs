using BioTonFMS.Telematica.Validation.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BioTonFMS.Infrastructure.Controllers
{
    public abstract class ValidationControllerBase : ControllerBase
    {
        public IActionResult ReturnValidationErrors(ValidationResult result)
        {
            result.AddToModelState(ModelState);
            return ValidationProblem(
                new ValidationProblemDetails(ModelState)
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = "/bad-request",
                    Title = "Запрос не соответствует требованиям API",
                    Detail = "Произошла ошибка валидации запроса"
                });
        }
    }
}
