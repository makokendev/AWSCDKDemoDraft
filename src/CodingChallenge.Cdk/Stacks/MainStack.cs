using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SAM;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.SQS;
using CodingChallenge.Cdk.Extensions;
using static Amazon.CDK.AWS.SAM.CfnFunction;

namespace CodingChallenge.Cdk.Stacks;

public sealed partial class MainStack : Stack
{
    public const string EcrRepoSuffix = "ecrrepo";
    public MainStack(Construct parent, string id, IStackProps props, AwsAppProject awsApplication) : base(parent, id, props)
    {
        var eventProcessorLambdaRoleSuffix = "lambdarole";

        var topicName = awsApplication.GetResourceName("blockChainEventTopic");
        var eventTopic = Topic.FromTopicArn(this, awsApplication.GetResourceName("blockChainEventTopic"), awsApplication.GetCfOutput($"{InfraStack.eventTopicSuffix}-{InfraStack.arnSuffixValue}"));
        SetupApigatewayToSns(awsApplication, eventTopic);
        var eventQueueStack = SetupEventTransactionQueue(awsApplication, eventTopic);
        var eventProcessorLambdaRole = new LambdaRoleNestedStack(this, awsApplication.GetResourceName($"{eventProcessorLambdaRoleSuffix}-stack"), new NestedStackProps(), awsApplication, eventProcessorLambdaRoleSuffix);
        SetupEventProcessorLambdaFunction(eventProcessorLambdaRole.RoleObj.RoleArn, awsApplication, eventTopic,eventQueueStack.QueueObj);
    }

    private QueueNestedStack SetupEventTransactionQueue(AwsAppProject awsApplication, ITopic eventTopic)
    {
        var blockChainEventQueueNameSuffix = "eventqueue";
        var blockChainEventQueueStackName = awsApplication.GetResourceName($"{blockChainEventQueueNameSuffix}-stack");
        var eventQueueStack = new QueueNestedStack(this, blockChainEventQueueStackName, new NestedStackProps(), awsApplication, blockChainEventQueueNameSuffix, isFifo: true, 10);
        eventTopic.AddSubscription(new SqsSubscription(eventQueueStack.QueueObj, new SqsSubscriptionProps()
        {
            RawMessageDelivery = true,
            // FilterPolicy = new Dictionary<string, SubscriptionFilter>(){
            //         {"EventName", SubscriptionFilter.StringFilter(new StringConditions(){
            //             MatchPrefixes = new string[]{"Transaction."}
            //         })
            //         }
            //     }
        }));
        return eventQueueStack;
    }

    private string GetDockerImageUri(AwsAppProject awsApplication)
    {
        var dockerImageName = awsApplication.GetCfOutput($"{InfraStack.EcrRepoSuffix}-name");
        var dockerImageEcrDomain = $"{this.Account}.dkr.ecr.{this.Region}.amazonaws.com";
        return $"{dockerImageEcrDomain}/{dockerImageName}:{awsApplication.Version}";
    }

    private CfnFunction SetupEventProcessorLambdaFunction(string roleArn, AwsAppProject awsApplication, ITopic topic, Queue eventQueue)
    {
        var repoUri = GetDockerImageUri(awsApplication);

        const string functionSuffix = "event-processor-lambda-func";

        var baseSettings = new LamdaFunctionCdkSettings
        {
            FunctionNameSuffix = functionSuffix.ToLower(),
            Memory = 512,
            ReservedConcurrentExecutions = 2,
            Timeout = 30,
            RoleArn = roleArn,
            ImageUri = repoUri,
            Architectures = new string[] { Amazon.CDK.AWS.Lambda.Architecture.ARM_64.Name }
        };
        var lambdaProp = baseSettings.GetLambdaContainerBaseProps(awsApplication);
       
        ((lambdaProp.Environment as FunctionEnvironmentProperty).Variables as Dictionary<string, string>).Add($"SNSTopicConfiguration__TopicRegion", "us-east-1");
        ((lambdaProp.Environment as FunctionEnvironmentProperty).Variables as Dictionary<string, string>).Add($"SNSTopicConfiguration__TopicArn", topic.TopicArn);
        lambdaProp.AddEventSourceProperty(eventQueue.GetQueueEventSourceProperty(10, true), "eventKey");

        return new Amazon.CDK.AWS.SAM.CfnFunction(this, lambdaProp.FunctionName, lambdaProp);

    }


    public Role GetApiGatewayRole(AwsAppProject cdkApp, string namesuffix) =>
        cdkApp.GetAPIGatewayRole(this, namesuffix)
        .AddCodeBuildReportGroupPolicy(cdkApp)
        .AddCloudWatchLogsPolicy(cdkApp)
        .AddCloudWatchLogGroupPolicy(cdkApp)
        .AddDynamoDBPolicy(cdkApp)
        .AddSnsPolicy(cdkApp)
        .AddSqsPolicy(cdkApp)
        .AddS3Policy(cdkApp)
        .AddSsmPolicy(cdkApp);

    public void SetupApigatewayToSns(AwsAppProject awsApplication, ITopic eventTopic)
    {
        var restApiName = awsApplication.GetResourceName("ingest");
        var api = new RestApi(this, restApiName, new RestApiProps
        {
            DeployOptions = new StageOptions()
            {
                StageName = "run",
                TracingEnabled = true,
            },
        });

        awsApplication.SetCfOutput(this, "apigwbase", api.Url);

        var topic = api.Root.AddResource("topic");
        var awsIntegration = new AwsIntegration(new AwsIntegrationProps
        {
            Service = "sns",
            Path = $"{this.Account}/{eventTopic.TopicName}",
            IntegrationHttpMethod = "POST",
            Options = new IntegrationOptions()
            {
                CredentialsRole = GetApiGatewayRole(awsApplication, "apigatewayrole"),
                PassthroughBehavior = PassthroughBehavior.NEVER,
                RequestParameters = new Dictionary<string, string>
                  {
                      {"integration.request.header.Content-Type", "'application/x-www-form-urlencoded'"}
                  },
                RequestTemplates = new Dictionary<string, string>
                    {
                  {"application/json", $"TopicArn={eventTopic.TopicArn}&MessageGroupId=uniqueid&Action=Publish&Message=$util.urlEncode(\"$method.request.querystring.message\")"}
                    },
                IntegrationResponses = new IntegrationResponse[]{

                    new IntegrationResponse(){
                         StatusCode =  "200",
                         ResponseTemplates = new Dictionary<string,string>(){
                              {"application/json", "{\"done\": true}"}
                         }
                    }
                }
            }
        });

        var method = topic.AddMethod("GET", awsIntegration, new MethodOptions()
        {
            MethodResponses = new MethodResponse[]{
                new MethodResponse(){
                    StatusCode = "200"
                }
            }
        });
    }
}

