using Amazon.CDK;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Cdk.Stacks;
using CodingChallenge.Infrastructure;
using System;

namespace CodingChallenge.Cdk;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        try
        {
            var awsApplication = app.GetAWSApplication();
            awsApplication.AwsRegion = app.Region;
            Console.WriteLine("subsystem is..." + awsApplication.Subsystem);

            Amazon.CDK.Environment makeEnv(string account = null, string region = null)
            {
                return new Amazon.CDK.Environment
                {
                    Account = account ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = region ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                };
            }
            var infraStack = new InfraStack(app, awsApplication.GetDefaultInfraStackName(), new StackProps() { Env = makeEnv() }, awsApplication);
            var mainStack = new MainStack(app, awsApplication.GetDefaultMainStackName(), new StackProps() { Env = makeEnv() }, awsApplication);
            var databaseStack = new DatabaseStack(app, awsApplication.GetDefaultDatabaseStackName(), new StackProps() { Env = makeEnv() }, awsApplication);
            SetStackTags(awsApplication, mainStack);
            SetStackTags(awsApplication, infraStack);
            SetStackTags(awsApplication, databaseStack);
            app.Synth();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"CDK EXCEPTION --- {ex.Message} -- {ex.StackTrace}");
            throw;
        }
    }

    private static void SetStackTags<T>(AWSAppProject awsApplication, T stack)
    where T : Stack
    {
        Tags.Of(stack).Add(nameof(awsApplication.Environment), awsApplication.Environment);
        Tags.Of(stack).Add(nameof(awsApplication.Version), awsApplication.Version);
        Tags.Of(stack).Add(nameof(awsApplication.Platform), awsApplication.Platform);
        Tags.Of(stack).Add(nameof(awsApplication.System), awsApplication.System);
        Tags.Of(stack).Add(nameof(awsApplication.Subsystem), awsApplication.Subsystem);
    }
}
