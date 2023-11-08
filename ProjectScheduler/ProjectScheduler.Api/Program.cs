using Infrastructure;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        IConfiguration config = GetConfig();
        string connectionString = config.GetConnectionString("db_connection");
        builder.Services.AddDbContext<AppDBContext>(option => option.UseNpgsql(connectionString,
            x => x.MigrationsAssembly("Infrastructure")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.Run();
    }

    private static IConfiguration GetConfig()
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.Development.json");

        return builder.Build();
    }
}