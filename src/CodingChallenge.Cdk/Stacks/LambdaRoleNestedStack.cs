using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using CodingChallenge.Cdk.Extensions;

namespace CodingChallenge.Cdk.Stacks;

public sealed class LambdaRoleNestedStack : Amazon.CDK.NestedStack
{
    public Role RoleObj { get; private set; }
    public LambdaRoleNestedStack(Construct parent, string id, Amazon.CDK.NestedStackProps props, AwsAppProject cdkApp, string namesuffix) : base(parent, id, props)
    {
        RoleObj = cdkApp.GetLambdaRole(this, namesuffix);
        RoleObj.AddCodeBuildReportGroupPolicy(cdkApp);
        RoleObj.AddCloudWatchLogsPolicy(cdkApp);
        RoleObj.AddCloudWatchLogGroupPolicy(cdkApp);
        RoleObj.AddDynamoDBPolicy(cdkApp);
        RoleObj.AddSnsPolicy(cdkApp);
        RoleObj.AddSqsPolicy(cdkApp);
        RoleObj.AddS3Policy(cdkApp);
        RoleObj.AddSsmPolicy(cdkApp);
    }
}
