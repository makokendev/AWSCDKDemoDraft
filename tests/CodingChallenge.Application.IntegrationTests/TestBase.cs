using CodingChallenge.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Application.IntegrationTests;

public class TestBase
{
    protected IConfiguration Configuration;
    protected ILogger Logger;
    protected ServiceProvider ServiceProvider;
    protected ISender Sender;

    protected virtual void Init()
    {
        SetConfiguration();
        ConfigureLogger();
        ConfigureServices(new ServiceCollection());
        Sender = ServiceProvider.GetService<ISender>();
    }
    protected virtual void SetConfiguration()
    {
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables().Build();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        var debugLogger = new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider().CreateLogger("testbase");
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(Configuration,debugLogger);
        services.AddSingleton<ILogger>(Logger);
        //services.AddTransient<NFTCommandController, NFTCommandController>();
        //services.AddTransient<NFTConsoleRunner, NFTConsoleRunner>();
        ServiceProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureLogger()
    {
        var loggerFactory = LoggerFactory.Create(
                builder => builder
                            // add debug output as logging target
                            .AddDebug()
                            // set minimum level to log
                            .SetMinimumLevel(LogLevel.Debug));
        Logger = loggerFactory.CreateLogger("ConsoleApp");
    }

}
