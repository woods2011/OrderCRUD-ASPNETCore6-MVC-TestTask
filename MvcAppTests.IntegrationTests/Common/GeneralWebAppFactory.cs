using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTests.IntegrationTests.Common;

public class GeneralWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient Client { get; private set; } = null!;
    

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddControllersAsServices();

            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddSingleton<IDbInitializer, TestDbInitializer>();
            
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });
            
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlite(connection: serviceProvider.GetRequiredService<DbConnection>());
            });
        });
        
        builder.UseEnvironment("Development"); 
    }

    public async Task InitializeAsync()
    {
        Client = CreateClient();
        using var serviceScope = Services.CreateScope();
        var dbCtx = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbCtx.Database.EnsureDeletedAsync();
        await dbCtx.Database.EnsureCreatedAsync();
    }
    
    public DbContextScopeHolder<AppDbContext> CreateDbContextScopeHolder() => new(Services);

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}

public class TestDbInitializer : IDbInitializer
{
    public void Seed(AppDbContext context)
    {
        var providers = Enumerable.Range(1, 20).Select(x => new Provider($"Provider {x}"));
        context.Providers.AddRange(providers);
        context.SaveChanges();
    }
}