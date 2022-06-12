using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingChallenge.Infrastructure.Persistence.NFTRecord;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration,ILogger logger)
    {
        services.AddAutoMapper(typeof(InfrastructureDependencyInjection).Assembly);
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            logger.LogDebug("opting for in memory database");
            services.AddDbContext<NFTRecordDataModelDbContext>(options =>
                options.UseInMemoryDatabase("NFTDatabase"));
        }
        else
        {
            logger.LogDebug("opting for sq lite database");
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dataSource = Path.Combine(assemblyFolder, "NFTDatabase.db");
            services.AddDbContext<NFTRecordDataModelDbContext>(options =>
                options.UseSqlite($"Data Source={dataSource};", options =>
                    {

                        options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    }));
        }
        services.AddNftEntityInfrastructure(configuration,logger);
        return services;
    }
}
