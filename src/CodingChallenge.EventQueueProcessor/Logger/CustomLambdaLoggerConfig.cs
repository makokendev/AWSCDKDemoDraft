using System;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.EventQueueProcessor.Logger;
public class CustomLambdaLoggerConfig
{
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public int EventId { get; set; } = 0;
    public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;
}


