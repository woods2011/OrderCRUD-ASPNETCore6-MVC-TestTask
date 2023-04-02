using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Providers;

public class GetProvidersSelectInfoQuery : IRequest<IReadOnlyList<ProviderSelectInfoVm>> { }

public class GetProvidersSelectInfoQueryHandler :
    IRequestHandler<GetProvidersSelectInfoQuery, IReadOnlyList<ProviderSelectInfoVm>>
{
    private readonly AppDbContext _context;

    public GetProvidersSelectInfoQueryHandler(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProviderSelectInfoVm>> Handle(
        GetProvidersSelectInfoQuery request,
        CancellationToken token)
    {
        var providerSelectInfoVm = await _context.Providers
            .Select(x => new ProviderSelectInfoVm(x.Id, x.Name))
            .ToListAsync(token);

        return providerSelectInfoVm;
    }
}

public record ProviderSelectInfoVm(int Id, string Name);