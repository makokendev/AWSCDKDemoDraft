using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using CodingChallenge.Application;
using CodingChallenge.EventQueueProcessor.Logger;
using CodingChallenge.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace CodingChallenge.EventQueueProcessor;
public class EventQueueLambdaClass
{
    public ILogger logger;
    public IConfiguration configuration;
    public IServiceProvider serviceProvider;

    public AWSAppProject awsApplication;

    public EventQueueLambdaClass()
    {
        LoadConfiguration();
        SetupLogger();
        ConfigureServices(new ServiceCollection());
    }

    protected virtual void LoadConfiguration()
    {
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables().Build();
        awsApplication = new AWSAppProject();
        configuration.GetSection(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX).Bind(awsApplication);
    }
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(configuration,logger);
        services.AddSingleton<ILogger>(logger);
        services.AddSingleton<AWSAppProject>(awsApplication);

        services.AddTransient<NFTRecordCommandController, NFTRecordCommandController>();
        services.AddTransient<NFTRecordLambdaRunner, NFTRecordLambdaRunner>();
        serviceProvider = services.BuildServiceProvider();
    }


    public void SetupLogger()
    {
        logger = new CustomLambdaLoggerProvider(new CustomLambdaLoggerConfig()
        {
            LogLevel = LogLevel.Debug,
            InfrastructureProject = awsApplication

        }).CreateLogger(nameof(EventQueueLambdaClass));
    }

    public async Task HandleAsync(SQSEvent sQSEvent, ILambdaContext context)
    {
        logger.LogInformation($"Handling SQS Event");
        if (sQSEvent == null || sQSEvent.Records == null || !sQSEvent.Records.Any())
        {
            logger.LogInformation($"No records are found");
            return;
        }
        var runner = serviceProvider.GetService<NFTRecordLambdaRunner>();

        foreach (var record in sQSEvent.Records)
        {
            try
            {
                logger.LogInformation($"log debug {record.Body}");
                await runner.HandleInlineJsonOptionAsync(record.Body);

            }

            catch (Exception ex)
            {
                logger.LogInformation($"error processing queue... Message: {ex.Message}. StackTrace {ex.StackTrace}. exception type -> {ex.GetType()}");
                logger.LogInformation($"inner exception error processing queue... Message: {ex.InnerException?.Message}. StackTrace {ex.InnerException?.StackTrace}");
                throw;
            }
        }
    }

}
