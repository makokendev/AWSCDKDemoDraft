using Amazon.CDK;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Cdk.Stacks;
using System;

namespace CodingChallenge.Cdk;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        try
        {
            var cdkApp = app.GetCdkApplication();
            Console.WriteLine("subsystem is..." + cdkApp.Subsystem);

            Amazon.CDK.Environment makeEnv(string account = null, string region = null)
            {
                return new Amazon.CDK.Environment
                {
                    Account = account ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = region ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                };
            }
            var infraStack = new InfraStack(app, cdkApp.GetDefaultInfraStackName(), new StackProps() { Env = makeEnv() }, cdkApp);
            var mainStack = new MainStack(app, cdkApp.GetDefaultMainStackName(), new StackProps() { Env = makeEnv() }, cdkApp);
            SetStackTags(cdkApp, mainStack);
            app.Synth();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"{ex.Message} -- {ex.StackTrace}");
            throw;
        }
    }

    private static void SetStackTags<T>(AwsAppProject cdkApp, T stack)
    where T : Stack
    {
        Tags.Of(stack).Add(nameof(cdkApp.Environment), cdkApp.Environment);
        Tags.Of(stack).Add(nameof(cdkApp.Version), cdkApp.Version);
        Tags.Of(stack).Add(nameof(cdkApp.Platform), cdkApp.Platform);
        Tags.Of(stack).Add(nameof(cdkApp.System), cdkApp.System);
        Tags.Of(stack).Add(nameof(cdkApp.Subsystem), cdkApp.Subsystem);
    }
}
