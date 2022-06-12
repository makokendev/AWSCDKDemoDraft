
using CodingChallenge.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;

public static class NFTRecordEntityDependencyInjection
{
    public static IServiceCollection AddNftEntityInfrastructure(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var databaseType = configuration.GetValue<string>(Constants.DATABASE_TYPE_ENV_VAR_KEY);
        if (!string.IsNullOrWhiteSpace(databaseType)
                && configuration.GetValue<string>(Constants.DATABASE_TYPE_ENV_VAR_KEY).Equals(Constants.DATABASE_TYPE_DYNAMODB_ENV_VAR_KEY))
        {
            logger.LogDebug("Using Dynamodb repository");
            services.AddScoped<INFTRecordRepository, NFTRecordDynamoDBRepository>();
        }
        else
        {
            logger.LogDebug("Using SQL LITE repository");
            services.AddScoped<INFTRecordRepository, NFTRecordRepository>();
        }
        return services;
    }
}
