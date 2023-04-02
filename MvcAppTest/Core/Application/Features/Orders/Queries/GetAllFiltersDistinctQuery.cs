using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public class GetAllFiltersDistinctQuery : IRequest<AllFiltersDistinctVm> { }

public class GetAllFiltersDistinctQueryHandler : IRequestHandler<GetAllFiltersDistinctQuery, AllFiltersDistinctVm>
{
    private readonly AppDbContext _context;

    public GetAllFiltersDistinctQueryHandler(AppDbContext context) => _context = context;

    public async Task<AllFiltersDistinctVm> Handle(GetAllFiltersDistinctQuery request, CancellationToken token)
    {
        var getAllFiltersDistinctVm = new AllFiltersDistinctVm(
            await _context.Orders.Select(o => o.Number).Distinct().OrderBy(o => o).ToListAsync(token),
            await _context.Orders.Select(o => o.Provider.Name).Distinct().OrderBy(o => o).ToListAsync(token),
            await _context.OrderItems.Select(oi => oi.Name).Distinct().OrderBy(o => o).ToListAsync(token),
            await _context.OrderItems.Select(oi => oi.Unit).Distinct().OrderBy(o => o).ToListAsync(token));

        return getAllFiltersDistinctVm;
    }
}

public record AllFiltersDistinctVm(
    List<string> OrderNumbers,
    List<string> OrderProviderNames,
    List<string> OrderItemNames,
    List<string> OrderItemUnits);


// _context.Orders
//     .GroupBy(order => order.Number)
//     .Select(g => new { g.First().Id, g.Key })
//     .OrderBy(o => o.Id).Select(o => o.Key)
//     .ToListAsync(token);