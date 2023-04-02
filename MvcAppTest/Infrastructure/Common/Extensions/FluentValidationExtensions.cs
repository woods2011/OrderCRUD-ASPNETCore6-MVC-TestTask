using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MvcAppTest.Infrastructure.Common.Extensions;

public static class FluentValidationExtensions
{
    public static void AddToModelState(this IEnumerable<ValidationFailure> errors, ModelStateDictionary modelState)
    {
        foreach (var error in errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }
}