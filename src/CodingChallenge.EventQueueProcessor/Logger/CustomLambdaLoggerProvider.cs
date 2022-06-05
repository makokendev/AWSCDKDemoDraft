using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.EventQueueProcessor.Logger;
public class CustomLambdaLoggerProvider : ILoggerProvider
{
    private readonly CustomLambdaLoggerConfig _config;
    private readonly ConcurrentDictionary<string, CustomLambdaLogger> _loggers = new ConcurrentDictionary<string, CustomLambdaLogger>();

    public CustomLambdaLoggerProvider(CustomLambdaLoggerConfig config)
    {
        _config = config;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CustomLambdaLogger(name, _config));
    }
    public void Dispose()
    {
        _loggers.Clear();
    }
}


