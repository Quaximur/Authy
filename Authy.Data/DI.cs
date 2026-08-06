using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Authy.Data;

public static class DI
{
    public static void AddDataServices(this IHostApplicationBuilder builder)
    {
        var dbContextConnectionString =
           Environment.GetEnvironmentVariable("ConnectionStrings__AuthyDbContext_Postgres")
           ?? builder.Configuration.GetConnectionString("AucyDbContext");

        builder.Services.AddDbContext<AuthyDbContext>(options =>
        {
            options.UseNpgsql(dbContextConnectionString);
        }, optionsLifetime: ServiceLifetime.Singleton);

        builder.Services.AddDbContextFactory<AuthyDbContext>(options =>
        {
            options.UseNpgsql(dbContextConnectionString);
        });
    }
}