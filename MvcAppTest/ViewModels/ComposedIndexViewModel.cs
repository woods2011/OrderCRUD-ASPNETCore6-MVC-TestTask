using FluentValidation;
using MvcAppTest.Core.Application.Common;
using MvcAppTest.Core.Application.Features.Orders.Queries;
using MvcAppTest.Infrastructure.Common.Extensions;

namespace MvcAppTest.ViewModels;

public class ComposedIndexViewModel
{
    public int Page { get; set; } = 1;

    public string[] OrdNumber { get; set; } = Array.Empty<string>();
    public string[] OrdProvider { get; set; } = Array.Empty<string>();

    public DateTime OrdStart { get; set; } = DateTime.Now.AddMonths(-1).RoundToDay();

    public DateTime OrdEnd { get; set; } = DateTime.Now.RoundToDay();

    public string[] OrdItemName { get; set; } = Array.Empty<string>();

    public string[] OrdItemUnit { get; set; } = Array.Empty<string>();
    
    public int FilterOn { get; set; } = 0;

    public PaginatedList<OrderVm> Orders { get; set; } = new(Array.Empty<OrderVm>(), 0, 1, 1);
}


public class ComposedIndexViewModelValidator : AbstractValidator<ComposedIndexViewModel>
{
    public ComposedIndexViewModelValidator()
    {
        RuleFor(vm => vm.OrdStart)
            .LessThanOrEqualTo(vm => vm.OrdEnd)
            .WithMessage("Дата начала интервала должна быть не больше даты окончания");
        
    }
}
