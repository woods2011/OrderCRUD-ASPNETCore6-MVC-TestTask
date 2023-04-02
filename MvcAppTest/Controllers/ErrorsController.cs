using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcAppTest.Core.Domain.Exceptions;
using MvcAppTest.ViewModels;

namespace MvcAppTest.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger) => _logger = logger;

    
    [Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var error = exceptionHandlerPathFeature?.Error;

        if (error is not null) errorViewModel.ErrorMessage = HandleException(error);
        return View(errorViewModel);
    }

    private string HandleException(Exception exception)
    {
        _logger.LogError(
            "An error occurred while processing user request. {@ErrorMessage}, {@Error}, {@DateTimeUtc}",
            exception.Message, exception, DateTime.UtcNow);

        return exception switch
        {
            ValidationException validationException => String.Join(" | ",
                validationException.Errors.Select(failure => failure.PropertyName + " : " + failure.ErrorMessage)),

            DomainException domainException => domainException.Message,
            _ => ""
        };
    }
}