using Microsoft.EntityFrameworkCore;
using WorkersService.Db;

namespace WorkersService.IoC;

public static class DbContextConfigurator
{
    public static void AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        
        services.AddDbContextFactory<WorkersDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        }, ServiceLifetime.Scoped);
    }
    
    public static void ConfigurePostgres(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<WorkersDbContext>>();
        using var context = contextFactory.CreateDbContext();
        context.Database.Migrate();
    }
}