using Microsoft.EntityFrameworkCore;
using ProjectSchedulerAuth.Application;
using ProjectSchedulerAuth.Application.Interfaces;
using ProjectSchedulerAuth.Domain.Repositories;
using ProjectSchedulerAuth.Infrastructure.DbTools;
using ProjectSchedulerAuth.Infrastructure.Repositories;
using ProjectSchedulerAuth.Infrastructure.Tools;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        var services = builder.Services;

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        services
            .AddDatabase(builder.Configuration)
            .AddRepositories()
            .AddServices();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with Scoped lifetime   
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("auth"),
                x => x.MigrationsAssembly("ProjectSchedulerAuth.Infrastructure"));
        });

        services.AddScoped<Func<AppDbContext>>((provider) => () => provider.GetService<AppDbContext>());
        services.AddScoped<DbFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        //.AddScoped<IDepartmentRepository, DepartmentRepository>()
        //.AddScoped<IUserRepository, UserRepository>()
        //.AddScoped<ISalaryRepository, SalaryRepository>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<Toolbox>();
    }
}