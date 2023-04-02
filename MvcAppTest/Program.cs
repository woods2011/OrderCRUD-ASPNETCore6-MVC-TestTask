using System.Globalization;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Behaviors;
using MvcAppTest.Core.Application.Common;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
    options.UseSqlServer(builder.Configuration["MvcAppTestConnectionString"]);
    // options.UseSqlServer("Server=localhost,2434;Database=MvcAppTestDb;User Id=sa;Password=!secret123");
});

builder.Services
    .AddOptions<PaginationOptions>()
    .Bind(builder.Configuration.GetSection(PaginationOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddLogging();

builder.Services.AddMapster();

builder.Services.AddFluentValidation();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();


var app = builder.Build();

ConfigureApplication(app);

app.Run();


static void ConfigureApplication(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    else app.UseDeveloperExceptionPage();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "OrderItems",
            pattern: "Orders/{orderId:int}/Items/{action=Index}/{id?}",
            defaults: new { controller = "OrderItems" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Orders}/{action=Index}/{id?}");
    });

    CreateAndSeedDbIfNotExists(app.Services);
}

static void CreateAndSeedDbIfNotExists(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();
        DbInitializer.SeedIfEmpty(context);
    }
}


#pragma warning disable CA1050
public partial class Program { }
#pragma warning restore CA1050


internal static class CompositionRootExtension
{
    public static void AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    public static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ru");
        services.Configure<MvcOptions>(options =>
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();
        builder.Logging.AddConsole();
        // builder.Logging.AddSerilog(logger);
    }
}

// .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false);