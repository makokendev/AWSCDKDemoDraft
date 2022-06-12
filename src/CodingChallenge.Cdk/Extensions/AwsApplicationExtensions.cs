using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SQS;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;

namespace CodingChallenge.Cdk.Extensions;

public static class AwsApplicationExtensions
{

    public static string GetDefaultMainStackName(this AWSAppProject awsApplication)
    {
        return awsApplication.GetResourceName(Constants.MAIN_STACK_NAME_SUFFIX);
    }
    public static string GetDefaultInfraStackName(this AWSAppProject awsApplication)
    {
        return awsApplication.GetResourceName(Constants.INFRA_STACK_NAME_SUFFIX);
    }
    public static string GetDefaultDatabaseStackName(this AWSAppProject awsApplication)
    {
        return awsApplication.GetResourceName(Constants.DATABASE_STACK_NAME_SUFFIX);
    }

    public static Queue GetSqsQueue<T>(this AWSAppProject awsApplication, T stack, string resourceSuffix, bool isFifo = false, IDeadLetterQueue deadLetterQueue = null)
       where T : Stack
    {
        string resourceName = awsApplication.GetResourceName(resourceSuffix);
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
    public static Role GetLambdaRole<T>(this AWSAppProject awsApplication, T stack, string resourceSuffix, bool isGlobal = false)
       where T : Stack
    {
        string resourceName = awsApplication.GetResourceName(resourceSuffix);
        var role = new Role(stack, resourceName, new RoleProps()
        {
            RoleName = resourceName,
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
        });
        return role;
    }


    public static Role GetAPIGatewayRole<T>(this AWSAppProject awsApplication, T stack, string resourceSuffix, bool isGlobal = false)
       where T : Stack
    {
        string resourceName = awsApplication.GetResourceName(resourceSuffix);
        var role = new Role(stack, resourceName, new RoleProps()
        {
            RoleName = resourceName,
            AssumedBy = new ServicePrincipal("apigateway.amazonaws.com"),
        });
        return role;
    }
   

    public static string SetCfOutput(this AWSAppProject awsApp, Stack stack, string exportSuffix, string exportValue)
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
    public static string GetCfOutput(this AWSAppProject awsApp, string exportSuffix)
    {
        var exportName = awsApp.ConstructCfExportName(exportSuffix);
        return Fn.ImportValue(exportName);
    }

    public static string ConstructCfExportName(this AWSAppProject awsApp, string exportSuffix)
    {
        return awsApp.GetResourceName(exportSuffix);
    }
    public static string SetApiGatewayCNameOutputValue(this AWSAppProject awsApp, Stack stack, string outputValue)
    {
        return awsApp.SetCfOutput(stack, Constants.REST_API_CNAME, outputValue);
    }



    public static Dictionary<string, string> GetEnvironmentVariables(this AWSAppProject awsApplication, string prefix)
    {
        return new Dictionary<string, string>{
                    { prefix + "Environment", awsApplication.Environment},
                    { prefix + "Platform", awsApplication.Platform},
                    { prefix + "System", awsApplication.System},
                    { prefix + "Subsystem",  awsApplication.Subsystem},
                    { prefix + "Version", awsApplication.Version},
                    { prefix + "AwsRegion", awsApplication.AwsRegion}, 
                };
    }
}