using System.Linq;
using System.Collections.Generic;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;

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
    public static string BuildDefaultAppResourceCondition(this AWSAppProject awsApplication, Stack stack, string serviceName, string policyPrefix)
    {
        return $"arn:aws:{serviceName}:{stack.Region}:{stack.Account}:{policyPrefix}*";
    }
    public static string IAMRolePolicyPrefix(this AWSAppProject app, string seperator = "-")
    {
        return $"{app.Environment}{seperator}{app.Platform}{seperator}{app.System}{seperator}{app.Subsystem}";
    }
    public static string GetAppResourceCondition(this AWSAppProject awsApplication, Stack stack, AwsResourceType resourceType)
    {
        switch (resourceType)
        {
            case AwsResourceType.SQS:
                {
                    return awsApplication.BuildDefaultAppResourceCondition(stack, "sqs", awsApplication.IAMRolePolicyPrefix());
                }

            case AwsResourceType.SNS:
                {
                    return awsApplication.BuildDefaultAppResourceCondition(stack, "sns", awsApplication.IAMRolePolicyPrefix());
                }

            case AwsResourceType.S3:
                {
                    return $"arn:aws:s3:::{awsApplication.Prefix()}*";
                }
            case AwsResourceType.SSMParameterStore:
                {
                    return $"arn:aws:ssm:{stack.Region}:{stack.Account}:parameter/{awsApplication.IAMRolePolicyPrefix("/")}*";
                }
            case AwsResourceType.DynamoDB:
                {
                    return $"arn:aws:dynamodb:{stack.Region}:{stack.Account}:table/{awsApplication.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CloudWatchLambdaLogs:
                {
                    return $"arn:aws:logs:{stack.Region}:{stack.Account}:log-group:/aws/lambda/{awsApplication.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CloudWatchLogGroup:
                {
                    return $"arn:aws:logs:{stack.Region}:{stack.Account}:log-group:/*{awsApplication.IAMRolePolicyPrefix()}*";
                }
            case AwsResourceType.CodeBuildReportGroup:
                {
                    return $"arn:aws:codebuild:{stack.Region}:{stack.Account}:report-group:/{awsApplication.IAMRolePolicyPrefix()}*";
                }

            default: break;
        }
        return null;
    }

    private static string[] GetProcessedResource(this Role role, AWSAppProject awsApplication, AwsResourceType resourceType, string[] conditions = null)
    {
        List<string> resources = new List<string>(){
                awsApplication.GetAppResourceCondition(role.Stack,resourceType)
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
    public static Role AddSnsPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.SNS, conditions);
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
    public static Role AddSsmPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.SSMParameterStore, conditions);
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
    public static Role AddSqsPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.SQS, conditions);
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
    public static Role AddS3Policy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.S3, conditions);
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
    public static Role AddCloudWatchLogsPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.CloudWatchLambdaLogs, conditions);
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
    public static Role AddCloudWatchLogGroupPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.CloudWatchLogGroup, conditions);
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
    public static Role AddCodeBuildReportGroupPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.CodeBuildReportGroup, conditions);
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
    public static Role AddDynamoDBPolicy(this Role role, AWSAppProject awsApplication, string[] conditions = null)
    {
        var processedResources = role.GetProcessedResource(awsApplication, AwsResourceType.DynamoDB, conditions);
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