using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MvcAppTest.Core.Domain.Exceptions;
using MvcAppTest.Infrastructure.Common.Extensions;

namespace MvcAppTest.Controllers;


public abstract class MvcController : Controller
{
    protected bool HandleDomainOrValidationException(Exception domainOrValidationException)
    {
        switch (domainOrValidationException)
        {
            case ValidationException validationException:
                validationException.Errors.AddToModelState(ModelState);
                return true;

            case DomainException domainException:
                ModelState.AddModelError(String.Empty, domainException.Message);
                return true;

            default: return false;
        }
    }
}