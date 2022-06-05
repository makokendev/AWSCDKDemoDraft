
using CodingChallenge.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;

public static class NFTRecordEntityDependencyInjection
{
    public static IServiceCollection AddNftEntityInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INFTRecordRepository, NFTRecordRepository>();
        return services;
    }
}
