using System.Collections.Generic;
using Amazon.CDK.AWS.SAM;
using Amazon.CDK.AWS.SQS;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;
using static Amazon.CDK.AWS.SAM.CfnFunction;

namespace CodingChallenge.Cdk.Extensions;

public static class CfnFunctionRelatedExtensions
{
    public static void AddEventSourceProperty<T>(this CfnFunctionProps functionProps, T eventSourceProperty, string eventKey)
       where T : EventSourceProperty
    {
        if (functionProps.Events == null)
        {
            functionProps.Events = new Dictionary<string, EventSourceProperty>();
        }
           (functionProps.Events as Dictionary<string, EventSourceProperty>).Add(eventKey, eventSourceProperty);
    }
    public static EventSourceProperty GetQueueEventSourceProperty(this Queue queue, int batchSize, bool enabled = false)
    {
        return new EventSourceProperty()
        {
            Type = "SQS",
            Properties = new SQSEventProperty()
            {
                Queue = queue.QueueArn,
                BatchSize = batchSize,
                Enabled = enabled
            }
        };
    }
    public static CfnFunctionProps GetLambdaContainerBaseProps(this LamdaFunctionCdkSettings functionCdkSettings, AWSAppProject awsApplication)
    {
        var functionName = awsApplication.GetResourceName(functionCdkSettings.FunctionNameSuffix);
        if (functionName.Length > 63)
        {
            functionName = functionName.Substring(0, 63);
        }

        var eventProps = new CfnFunctionProps()
        {
            PackageType = "Image",
            FunctionName = functionName,
            Timeout = functionCdkSettings.Timeout,
            ReservedConcurrentExecutions = functionCdkSettings.ReservedConcurrentExecutions,
            MemorySize = functionCdkSettings.Memory,
            //AutoPublishAlias = options.Version,
            Environment = new FunctionEnvironmentProperty()
            {
                Variables = awsApplication.GetEnvironmentVariables(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX + "__")
            },
            Role = functionCdkSettings.RoleArn,
            ImageUri = functionCdkSettings.ImageUri,
            Architectures = functionCdkSettings.Architectures
        };
        if (!string.IsNullOrWhiteSpace(functionCdkSettings?.HandlerClassType?.Namespace)
        && !string.IsNullOrWhiteSpace(functionCdkSettings?.HandlerClassType?.FullName)
        && !string.IsNullOrWhiteSpace(functionCdkSettings?.HandlerFunctionName))
        {
            eventProps.ImageConfig = new Amazon.CDK.AWS.SAM.CfnFunction.ImageConfigProperty()
            {
                Command = new string[] { $"{functionCdkSettings.HandlerClassType.Namespace}::{functionCdkSettings.HandlerClassType.FullName}::{functionCdkSettings.HandlerFunctionName}" },
            };
        }
        return eventProps;
    }
}
