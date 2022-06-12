using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SNS;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;
using CodingChallenge.Infrastructure.Persistence.NFTRecord;
using Newtonsoft.Json;

namespace CodingChallenge.Cdk.Stacks;

public sealed class ApiGatewayNestedStack : Amazon.CDK.NestedStack
{
    public ApiGatewayNestedStack(Construct parent, string id, Amazon.CDK.NestedStackProps props, AWSAppProject awsApplication, string eventTopicArn, string restApiid, string restApiRootResourceId, string roleArn) : base(parent, id, props)
    {
        var eventTopic = Topic.FromTopicArn(this, awsApplication.GetResourceName("blockChainEventTopic"), eventTopicArn);
        var apigwRole = Role.FromRoleArn(this, "apigwrole", roleArn);

        var api = RestApi.FromRestApiAttributes(this, "restapi", new RestApiAttributes()
        {
            RestApiId = restApiid,
            RootResourceId = restApiRootResourceId
        });

        
        var apiDeployment =new Deployment(this, "new deployment", new DeploymentProps
        {
            Api = api,
            Description = "new sns, wallet and token resources",
            RetainDeployments = true,

        });
        apiDeployment.AddToLogicalId(DateTime.UtcNow.ToLongDateString()); //need force change deployment hash to force new deployment
        apiDeployment.Node.AddDependency(SetSnsEndpoint(awsApplication, api, apigwRole, eventTopic));
        apiDeployment.Node.AddDependency(SetWalletEndpoint(awsApplication, api, apigwRole));
        apiDeployment.Node.AddDependency(SetTokenEndpoint(awsApplication, api, apigwRole));
        //  if the 'stageName' already exists (from the core apigateway deployment) then the existing stage will be used !
    }


    private Method SetWalletEndpoint(AWSAppProject awsApplication, IRestApi api, IRole apigwRole)
    {
        var walletIntegration = GetDynamoDbWalletQueryIntegration(awsApplication, apigwRole);
        var token = api.Root.AddResource("wallet");
        return token.AddMethod("GET", walletIntegration, new MethodOptions()
        {
            MethodResponses = new MethodResponse[]{
                new MethodResponse(){
                    StatusCode = "200"
                }
            }
        });
    }
    private Method SetTokenEndpoint(AWSAppProject awsApplication, IRestApi api, IRole apigwRole)
    {
        var walletIntegration = GetDynamoDbTokenScanIntegration(awsApplication, apigwRole);
        var token = api.Root.AddResource("token");
        return token.AddMethod("GET", walletIntegration, new MethodOptions()
        {
            MethodResponses = new MethodResponse[]{
                new MethodResponse(){
                    StatusCode = "200"
                }
            }
        });
    }

    private Method SetSnsEndpoint(AWSAppProject awsApplication, IRestApi api, IRole apigwRole, ITopic eventTopic)
    {
        var topic = api.Root.AddResource("topic");
        var snsIntegration = GetSnsIntegration(awsApplication, eventTopic, apigwRole);
        return topic.AddMethod("GET", snsIntegration, new MethodOptions()
        {
            MethodResponses = new MethodResponse[]{
                new MethodResponse(){
                    StatusCode = "200"
                }
            }
        });
    }

    private AwsIntegration GetSnsIntegration(AWSAppProject awsApplication, ITopic eventTopic, IRole apigwRole)
    {
        return new AwsIntegration(new AwsIntegrationProps
        {
            Service = "sns",
            Path = $"{this.Account}/{eventTopic.TopicName}",
            IntegrationHttpMethod = "POST",
            Options = new IntegrationOptions()
            {
                CredentialsRole = apigwRole,
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
    }
    private AwsIntegration GetDynamoDbGetItemIntegration(AWSAppProject awsApplication, IRole apigwRole)
    {
        var getItemRequestTemplate = new
        {
            Key = new
            {
                TokenId = new
                {
                    S = "$method.request.querystring.id"
                },
                WalletId = new
                {
                    S = "$method.request.querystring.walletid"
                }
            },
            TableName = awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel))
        };

        return new AwsIntegration(new AwsIntegrationProps
        {
            Service = "dynamodb",
            Action = "GetItem",
            IntegrationHttpMethod = "POST",
            Options = new IntegrationOptions()
            {
                CredentialsRole = apigwRole,
                PassthroughBehavior = PassthroughBehavior.NEVER,
                // RequestParameters = new Dictionary<string, string>
                //   {
                //       {"integration.request.header.Content-Type", "'application/x-www-form-urlencoded'"}
                //   },
                RequestTemplates = new Dictionary<string, string>
                    {
                  {"application/json", JsonConvert.SerializeObject(getItemRequestTemplate)}
                    },
                IntegrationResponses = new IntegrationResponse[]{

                    new IntegrationResponse(){
                         StatusCode =  "200",
                        //  ResponseTemplates = new Dictionary<string,string>(){
                        //       {"application/json", "{\"done\": true}"}
                        //  }
                    }
                }
            }
        });
    }
    private AwsIntegration GetDynamoDbWalletQueryIntegration(AWSAppProject awsApplication, IRole apigwRole)
    {
        var getItemRequestTemplate = new
        {
            Key = new
            {
                TokenId = new
                {
                    S = "$method.request.querystring.id"
                },
                WalletId = new
                {
                    S = "$method.request.querystring.walletid"
                }
            },
            TableName = awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel))
        };
        var queryRequestTemplate = new Dictionary<string, object>{
            {"TableName",awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel))},
            {"KeyConditionExpression","WalletId = :c"},
            {"ExpressionAttributeValues",new Dictionary<string,object>{
                {":c",new Dictionary<string,object>{
                    {"S","$method.request.querystring.walletid"}
                }}
            }}
        };

        return new AwsIntegration(new AwsIntegrationProps
        {
            Service = "dynamodb",
            Action = "Query",
            IntegrationHttpMethod = "POST",
            Options = new IntegrationOptions()
            {
                CredentialsRole = apigwRole,
                PassthroughBehavior = PassthroughBehavior.NEVER,
                // RequestParameters = new Dictionary<string, string>
                //   {
                //       {"integration.request.header.Content-Type", "'application/x-www-form-urlencoded'"}
                //   },
                RequestTemplates = new Dictionary<string, string>
                    {
                  {"application/json", JsonConvert.SerializeObject(queryRequestTemplate)}
                    },
                IntegrationResponses = new IntegrationResponse[]{

                    new IntegrationResponse(){
                         StatusCode =  "200",
                        //  ResponseTemplates = new Dictionary<string,string>(){
                        //       {"application/json", "{\"done\": true}"}
                        //  }
                    }
                }
            }
        });
    }
    private AwsIntegration GetDynamoDbTokenScanIntegration(AWSAppProject awsApplication, IRole apigwRole)
    {
        var getItemRequestTemplate = new
        {
            Key = new
            {
                TokenId = new
                {
                    S = "$method.request.querystring.id"
                },
                WalletId = new
                {
                    S = "$method.request.querystring.walletid"
                }
            },
            TableName = awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel))
        };
        var scanRequestExample = new Dictionary<string, object>{
            {"TableName",awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel))},
            {"FilterExpression","TokenId = :c"},
            {"ExpressionAttributeValues",new Dictionary<string,object>{
                {":c",new Dictionary<string,object>{
                    {"S","$method.request.querystring.id"}
                }}
            }}
        };

        return new AwsIntegration(new AwsIntegrationProps
        {
            Service = "dynamodb",
            Action = "Scan",
            IntegrationHttpMethod = "POST",
            Options = new IntegrationOptions()
            {
                CredentialsRole = apigwRole,
                PassthroughBehavior = PassthroughBehavior.NEVER,
                // RequestParameters = new Dictionary<string, string>
                //   {
                //       {"integration.request.header.Content-Type", "'application/x-www-form-urlencoded'"}
                //   },
                RequestTemplates = new Dictionary<string, string>
                    {
                  {"application/json", JsonConvert.SerializeObject(scanRequestExample)}
                    },
                IntegrationResponses = new IntegrationResponse[]{

                    new IntegrationResponse(){
                         StatusCode =  "200",
                        //  ResponseTemplates = new Dictionary<string,string>(){
                        //       {"application/json", "{\"done\": true}"}
                        //  }
                    }
                }
            }
        });
    }
}

