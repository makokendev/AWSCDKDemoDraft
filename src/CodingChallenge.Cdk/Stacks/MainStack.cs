using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SNS;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;
using static Amazon.CDK.AWS.APIGateway.CfnDomainName;

namespace CodingChallenge.Cdk.Stacks;

public sealed partial class MainStack : Stack
{
    public MainStack(Construct parent, string id, IStackProps props, AWSAppProject awsApplication) : base(parent, id, props)
    {
        if (string.IsNullOrWhiteSpace(awsApplication.AwsRegion))
        {
            awsApplication.AwsRegion = this.Region;
        }

        ITopic eventTopic = GetTopicReference(awsApplication);
        SetupEventProcessor(awsApplication, eventTopic);
        SetupApiGateway(awsApplication, eventTopic);
    }

    private ITopic GetTopicReference(AWSAppProject awsApplication)
    {
        var topicName = awsApplication.GetResourceName("blockChainEventTopic");
        var eventTopic = Topic.FromTopicArn(this, awsApplication.GetResourceName("blockChainEventTopic"), awsApplication.GetCfOutput($"{InfraStack.eventTopicSuffix}-{InfraStack.arnSuffixValue}"));
        return eventTopic;
    }

    private void SetupEventProcessor(AWSAppProject awsApplication, ITopic eventTopic)
    {
        var eventProcessorLambdaRoleSuffix = "lambdarole";
        var eventProcessorLambdaRole = GetLambdaRole(awsApplication, $"{eventProcessorLambdaRoleSuffix}");
        new EventProcessorNestedStack(this, "eventprocessor", new NestedStackProps(), awsApplication, eventProcessorLambdaRole.RoleArn, eventTopic.TopicArn);
    }

    private void SetupApiGateway(AWSAppProject awsApplication, ITopic eventTopic)
    {
        var apiGw = GetApiGateway(awsApplication);
        var apiGwRole = GetApiGatewayRole(awsApplication, "apigatewayrole");
        
        //this function has to be run after api gw is created. CloudFormation is not always straight-forward.
        //uncomment after api gw is created.
        //SetCustomDomainForApiGateway(awsApplication,apiGw);
        new ApiGatewayNestedStack(this, "ApiGateway", new NestedStackProps(), awsApplication, eventTopic.TopicArn, apiGw.RestApiId, apiGw.Root.ResourceId, apiGwRole.RoleArn);
    }
    private void SetCustomDomainForApiGateway(AWSAppProject awsApplication,RestApi restapi)
    {
        var domainNameObj = new Amazon.CDK.AWS.APIGateway.CfnDomainName(this, awsApplication.GetResourceName($"{restapi.RestApiName}-domainname"), new CfnDomainNameProps()
        {
            //Certificate = certificate,
            RegionalCertificateArn = awsApplication.CertificateArn,
            DomainName = $"awscdkapi.{awsApplication.DomainName}",
            EndpointConfiguration = new EndpointConfigurationProperty()
            {
                Types = new string[] { "REGIONAL" }
            }
        });
        var basePathMapping = new CfnBasePathMapping(this, awsApplication.GetResourceName($"{restapi.RestApiName}-basemappings"), new CfnBasePathMappingProps()
        {
            DomainName = domainNameObj.DomainName,
            RestApiId = restapi.RestApiId,
            Stage = awsApplication.Environment
        });
        basePathMapping.AddDependsOn(domainNameObj);
    }

    private RestApi GetApiGateway(AWSAppProject awsApplication)
    {

        var restApiName = awsApplication.GetResourceName("ingest");
        var api = new RestApi(this, restApiName, new RestApiProps
        {
            DeployOptions = new StageOptions()
            {
                StageName =awsApplication.Environment,
                TracingEnabled = true,
            },
        });

        var mockedResource = api.Root.AddResource("version", new ResourceOptions
        {
            DefaultCorsPreflightOptions = new CorsOptions
            {
                AllowOrigins = new string[] { "*" },
                AllowCredentials = true
            }
        });

        mockedResource.AddMethod("GET", new MockIntegration(new IntegrationOptions()
        {
            PassthroughBehavior = PassthroughBehavior.WHEN_NO_TEMPLATES,
            RequestTemplates = new Dictionary<string, string>
                    {
                  {"application/json", "{\"statusCode\": 200}"}
                  },
            IntegrationResponses = new IntegrationResponse[]{

                    new IntegrationResponse(){
                         StatusCode =  "200",
                         ResponseTemplates = new Dictionary<string,string>(){
                              {"application/json", $"{{\"version\": {awsApplication.Version}}}"}
                         }
                    }
                }
        }), new MethodOptions()
        {
            MethodResponses = new MethodResponse[]{
                new MethodResponse{
                    StatusCode = "200"
                }
            }
        });
        awsApplication.SetCfOutput(this, "apigwbase", api.Url);
        return api;
    }
    public Role GetApiGatewayRole(AWSAppProject awsApplication, string namesuffix) =>
      awsApplication.GetAPIGatewayRole(this, namesuffix)
      .AddCodeBuildReportGroupPolicy(awsApplication)
      .AddCloudWatchLogsPolicy(awsApplication)
      .AddCloudWatchLogGroupPolicy(awsApplication)
      .AddDynamoDBPolicy(awsApplication)
      .AddSnsPolicy(awsApplication)
      .AddSqsPolicy(awsApplication)
      .AddS3Policy(awsApplication)
      .AddSsmPolicy(awsApplication);
    public Role GetLambdaRole(AWSAppProject awsApplication, string namesuffix) =>
      awsApplication.GetLambdaRole(this, namesuffix)
      .AddCodeBuildReportGroupPolicy(awsApplication)
      .AddCloudWatchLogsPolicy(awsApplication)
      .AddCloudWatchLogGroupPolicy(awsApplication)
      .AddDynamoDBPolicy(awsApplication)
      .AddSnsPolicy(awsApplication)
      .AddSqsPolicy(awsApplication)
      .AddS3Policy(awsApplication)
      .AddSsmPolicy(awsApplication);
}

