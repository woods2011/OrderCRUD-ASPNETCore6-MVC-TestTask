using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MvcAppTests.IntegrationTests.Common;

public class DbContextScopeHolder<TContext> : IAsyncDisposable where TContext : DbContext
{
    private readonly AsyncServiceScope _scope;
    private readonly Lazy<TContext> _lazyDbContext;
    public TContext DbCtx => _lazyDbContext.Value;

    public DbContextScopeHolder(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateAsyncScope();
        _lazyDbContext = new Lazy<TContext>(CreateDbContext);
    }

    public DbContextScopeHolder(IServiceScopeFactory serviceScopeFactory)
    {
        _scope = serviceScopeFactory.CreateAsyncScope();
        _lazyDbContext = new Lazy<TContext>(CreateDbContext);
    }

    private TContext CreateDbContext()
    {
        var dbContext = _scope.ServiceProvider.GetRequiredService<TContext>();
        return dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}