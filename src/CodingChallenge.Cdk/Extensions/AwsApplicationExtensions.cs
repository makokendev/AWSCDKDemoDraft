using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SQS;

namespace CodingChallenge.Cdk.Extensions;

public static class AwsApplicationExtensions
{

    public static string GetDefaultMainStackName(this AwsAppProject cdkApp)
    {
        return cdkApp.GetResourceName(Constants.MAIN_STACK_NAME_SUFFIX);
    }
    public static string GetDefaultInfraStackName(this AwsAppProject cdkApp)
    {
        return cdkApp.GetResourceName(Constants.INFRA_STACK_NAME_SUFFIX);
    }

    public static Queue GetSqsQueue<T>(this AwsAppProject cdkApp, T stack, string resourceSuffix, bool isFifo = false, IDeadLetterQueue deadLetterQueue = null)
       where T : Stack
    {
        string resourceName = cdkApp.GetResourceName(resourceSuffix);
        if (isFifo)
        {
            resourceName += ".fifo";
        }
        var props = new QueueProps()
        {
            Fifo = isFifo,
            QueueName = resourceName,
            RetentionPeriod = Duration.Days(10),
            //VisibilityTimeout = Duration.Seconds(0),
            ReceiveMessageWaitTime = Duration.Seconds(0),
            DeadLetterQueue = deadLetterQueue,
        };
        if (isFifo)
        {
            props.ContentBasedDeduplication = true;
        }
        var resource = new Queue(stack, resourceName, props);
        return resource;
    }
    public static Role GetLambdaRole<T>(this AwsAppProject cdkApp, T stack, string resourceSuffix, bool isGlobal = false)
       where T : Stack
    {
        string resourceName = cdkApp.GetResourceName(resourceSuffix);
        var role = new Role(stack, resourceName, new RoleProps()
        {
            RoleName = resourceName,
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
        });
        return role;
    }


    public static Role GetAPIGatewayRole<T>(this AwsAppProject cdkApp, T stack, string resourceSuffix, bool isGlobal = false)
       where T : Stack
    {
        string resourceName = cdkApp.GetResourceName(resourceSuffix);
        var role = new Role(stack, resourceName, new RoleProps()
        {
            RoleName = resourceName,
            AssumedBy = new ServicePrincipal("apigateway.amazonaws.com"),
        });
        return role;
    }
    public static string Prefix(this AwsAppProject app, string seperator = "-")
    {
        return $"{app.Environment}{seperator}{app.Platform}{seperator}{app.System}{seperator}{app.Subsystem}";
    }
    public static string GetResourceName(this AwsAppProject cdkApp, string resourceSuffix)
    {
        string resourceName;

        resourceName = $"{cdkApp.Prefix()}-{resourceSuffix}";
        return resourceName;
    }

    public static string SetCfOutput(this AwsAppProject awsApp, Stack stack, string exportSuffix, string exportValue)
    {
        var exportName = awsApp.ConstructCfExportName(exportSuffix);
        var cfOutputBaseMappindgProps = new CfnOutputProps()
        {
            ExportName = exportName,
            Value = exportValue
        };
        _ = new CfnOutput(stack, exportName, cfOutputBaseMappindgProps);
        return exportName;
    }
    public static string GetCfOutput(this AwsAppProject awsApp, string exportSuffix)
    {
        var exportName = awsApp.ConstructCfExportName(exportSuffix);
        return Fn.ImportValue(exportName);
    }

    public static string ConstructCfExportName(this AwsAppProject awsApp, string exportSuffix)
    {
        return awsApp.GetResourceName(exportSuffix);
    }
    public static string SetApiGatewayCNameOutputValue(this AwsAppProject awsApp, Stack stack, string outputValue)
    {
        return awsApp.SetCfOutput(stack, Constants.REST_API_CNAME, outputValue);
    }



    public static Dictionary<string, string> GetEnvironmentVariables(this AwsAppProject cdkApp, string prefix)
    {
        return new Dictionary<string, string>{
                    { prefix + "Environment", cdkApp.Environment},
                    { prefix + "Platform", cdkApp.Platform},
                    { prefix + "System", cdkApp.System},
                    { prefix + "Subsystem",  cdkApp.Subsystem},
                    { prefix + "Version", cdkApp.Version},
                };
    }
}