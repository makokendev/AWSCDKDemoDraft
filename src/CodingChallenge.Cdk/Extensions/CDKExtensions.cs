using Amazon.CDK;
using CodingChallenge.Infrastructure;

namespace CodingChallenge.Cdk.Extensions;

public static class CDKExtensions
{
    public static AWSAppProject GetAWSApplication(this App app)
    {
        var awsApplication = new AWSAppProject
        {
            Platform = app.Node.TryGetContext("Platform").ToString(),
            System = app.Node.TryGetContext("System").ToString(),
            Subsystem = app.Node.TryGetContext("Subsystem").ToString(),
            Environment = app.Node.TryGetContext("Environment").ToString(),
            Version = app.Node.TryGetContext("Version").ToString(),
            CertificateArn = app.Node.TryGetContext("CertificateArn").ToString(),
            DomainName = app.Node.TryGetContext("DomainName").ToString()
        };
        return awsApplication;
    }
}
