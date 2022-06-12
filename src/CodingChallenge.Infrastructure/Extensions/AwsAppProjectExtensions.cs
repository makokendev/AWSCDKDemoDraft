using System;

namespace CodingChallenge.Infrastructure.Extensions;

public static class AwsAppProjectExtensions
{
    public static string Prefix(this AWSAppProject app, string seperator = "-")
    {
        return $"{app.Environment}{seperator}{app.Platform}{seperator}{app.System}{seperator}{app.Subsystem}";
    }
    public static string GetResourceName(this AWSAppProject awsApplication, string resourceSuffix)
    {
        string resourceName;

        resourceName = $"{awsApplication.Prefix()}-{resourceSuffix}";
        return resourceName;
    }
    public static string GetDynamodbTableName(this AWSAppProject awsApplication, Type dataModeType)
    {
        return awsApplication.GetResourceName(dataModeType.Name.ToLowerInvariant());
    }
}