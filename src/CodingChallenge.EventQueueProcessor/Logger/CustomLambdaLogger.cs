using System;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.EventQueueProcessor.Logger;

public class CustomLambdaLogger : ILogger
{
    private readonly string _name;
    private readonly CustomLambdaLoggerConfig _config;

    public CustomLambdaLogger(string name, CustomLambdaLoggerConfig config)
    {
        _name = name;
        _config = config;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == _config.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        Amazon.Lambda.Core.LambdaLogger.Log(message: $"{logLevel} - {eventId.Id} - {_name} - {formatter(state, exception)}");

    }
}


