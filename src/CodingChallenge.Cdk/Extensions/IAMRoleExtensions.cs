using System.Linq;
using System.Collections.Generic;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK;

namespace CodingChallenge.Cdk.Extensions;
public enum AwsResourceType
{
    S3,
    SQS,
    SNS,
    SSMParameterStore,
    CloudWatchLambdaLogs,
    CloudWatchLogGroup,
    CodeBuildReportGroup,
    DynamoDB

}
public static class IAMRoleExtensions
{
    public static string BuildDefaultAppResourceCondition(this AwsAppProject cdkApp, Stack stack, string serviceName, string policyPrefix)
    {
        return $"arn:aws:{serviceName}:{stack.Region}:{stack.Account}:{policyPrefix}*";
    }
    public static string IAMRolePolicyPrefix(this AwsAppProject app, string seperator = "-")
    {
        return $"{app.Environment}{seperator}{app.Platform}{seperator}{app.System}{seperator}{app.Subsystem}";
    }
    public static string GetAppResourceCondition(this AwsAppProject cdkApp, Stack stack, AwsResourceType resourceType)
    {
        switch (resourceType)
        {
            case AwsResourceType.SQS:
                {
                    return cdkApp.BuildDefaultAppResourceCondition(stack, "sqs", cdkApp.IAMRolePolicyPrefix());
                }

            case AwsResourceType.SNS:
                {
                    return cdkApp.BuildDefaultAppResourceCondition(stack, "sns", cdkApp.IAMRolePolicyPrefix());
                }

            case AwsResourceType.S3:
                {
                    return $"arn:aws:s3:::{cdkApp.Prefix()}*";
                }
            case AwsResourceType.SSMParameterStore:
                {
                    return $"arn:aws:ssm:{stack.Region}:{stack.Account}:parameter/{cdkApp.IAMRolePolicyPrefix("/")}*";
                }
            case AwsResourceType.DynamoDB:
                {
                    return $"arn:aws:dynamodb:{stack.Region}:{stack.Account}:table/{cdkApp.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CloudWatchLambdaLogs:
                {
                    return $"arn:aws:logs:{stack.Region}:{stack.Account}:log-group:/aws/lambda/{cdkApp.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CloudWatchLogGroup:
                {
                    return $"arn:aws:logs:{stack.Region}:{stack.Account}:log-group:/*{cdkApp.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CodeBuildReportGroup:
                {
                    return $"arn:aws:codebuild:{stack.Region}:{stack.Account}:report-group:/{cdkApp.IAMRolePolicyPrefix()}*";
                }

            default: break;
        }
        return null;
    }

    private static string[] GetProcessedResource(this Role role, AwsAppProject cdkApp, AwsResourceType resourceType, string[] conditions = null)
    {
        List<string> resources = new List<string>(){
                cdkApp.GetAppResourceCondition(role.Stack,resourceType)
            };
        if (conditions?.Any() == true)
        {
            resources.AddRange(conditions);
        }
        return resources.ToArray();
    }
    public static Role AddManagedPolicy(this Role role, string managedPolicyName)
    {
        var managedPolicy = ManagedPolicy.FromAwsManagedPolicyName(managedPolicyName);
        role.AddManagedPolicy(managedPolicy);
        return role;
    }
    public static Role AddSnsPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.SNS, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{"sns:Publish",
                                        "sns:Subscribe",
                                        "sns:Unsubscribe"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddSsmPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.SSMParameterStore, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                    "ssm:GetParameter*"
                },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddSqsPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.SQS, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{"sqs:ChangeMessageVisibility"
                                        ,"sqs:DeleteMessage"
                                        ,"sqs:GetQueue*"
                                        ,"sqs:PurgeQueue"
                                        ,"sqs:ReceiveMessage"
                                        ,"sqs:SendMessage"
                                        ,"sqs:SendMessageBatch"},
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddS3Policy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.S3, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                                        "s3:Get*",
                                        "s3:List*",
                                        "s3:DeleteObject*",
                                        "s3:PutObject*"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddCloudWatchLogsPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.CloudWatchLambdaLogs, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                                        "logs:CreateLogStream",
                                        "logs:DescribeLogStreams",
                                        "logs:PutLogEvents"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddCloudWatchLogGroupPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.CloudWatchLogGroup, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                                      "logs:CreateLogGroup"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddCodeBuildReportGroupPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.CodeBuildReportGroup, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                                        "codebuild:CreateReportGroup",
                                        "codebuild:CreateReport",
                                        "codebuild:UpdateReport",
                                        "codebuild:BatchPutTestCases"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }
    public static Role AddDynamoDBPolicy(this Role role, AwsAppProject cdkApp, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(cdkApp, AwsResourceType.DynamoDB, conditions);
        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps()
        {
            Actions = new string[]{
                                       "dynamodb:BatchGet*",
                                        "dynamodb:DescribeStream",
                                        "dynamodb:DescribeTable",
                                        "dynamodb:Get*",
                                        "dynamodb:Query",
                                        "dynamodb:Scan",
                                        "dynamodb:BatchWrite*",
                                        "dynamodb:DeleteItem",
                                        "dynamodb:UpdateItem",
                                        "dynamodb:PutItem"
                                        },
            Effect = Effect.ALLOW,
            Resources = processedResources
        }));
        return role;
    }

}