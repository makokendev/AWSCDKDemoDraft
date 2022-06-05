using Amazon.CDK;

namespace CodingChallenge.Cdk.Extensions;

public static class CDKExtensions
{
    public static AwsAppProject GetCdkApplication(this App app)
    {
        var cdkApp = new AwsAppProject
        {
            Platform = app.Node.TryGetContext("Platform").ToString(),
            System = app.Node.TryGetContext("System").ToString(),
            Subsystem = app.Node.TryGetContext("Subsystem").ToString(),
            Environment = app.Node.TryGetContext("Environment").ToString(),
            Version = app.Node.TryGetContext("Version").ToString()
        };
        return cdkApp;
    }
}
