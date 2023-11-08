using Microsoft.EntityFrameworkCore;
using ProjectSchedulerAuth.Application;
using ProjectSchedulerAuth.Application.Interfaces;
using ProjectSchedulerAuth.Domain.Repositories;
using ProjectSchedulerAuth.Infrastructure.DbTools;
using ProjectSchedulerAuth.Infrastructure.Repositories;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        services.AddControllers();
        services
            .AddDatabase(builder.Configuration)
            .AddRepositories()
            .AddServices();

        var app = builder.Build();

        app.UseRouting();

        app.UseAuthorization();

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
            options.UseNpgsql(configuration.GetConnectionString("auth"));
        });

        services.AddScoped<Func<AppDbContext>>((provider) => () => provider.GetService<AppDbContext>());
        services.AddScoped<DbFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //.AddScoped<IDepartmentRepository, DepartmentRepository>()
            //.AddScoped<IUserRepository, UserRepository>()
            //.AddScoped<ISalaryRepository, SalaryRepository>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthService, AuthService>();
    }
}