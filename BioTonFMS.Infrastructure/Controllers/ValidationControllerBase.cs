using BioTonFMS.Infrastructure.Services;
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
            var errorResult = new ServiceErrorResult();
            foreach (var error in result.Errors)
            {
                errorResult.AddError(error.ErrorMessage);
            }
            return BadRequest(errorResult);
        }
    }
}
