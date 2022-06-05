using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.SNS;
using CodingChallenge.Cdk.Extensions;

namespace CodingChallenge.Cdk.Stacks;

public sealed class InfraStack : Stack
{
    public const string EcrRepoSuffix = "ecrrepo";
    public const string eventTopicSuffix = "eventTopic";
    public const string dynamoDBTableSuffix = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel);
    public const string arnSuffixValue = "arn";
    public InfraStack(Construct parent, string id, IStackProps props, AwsAppProject awsApplication) : base(parent, id, props)
    {
        SetupTableForNFTRecord(awsApplication);
        CreateECRRepoForEventProcessor(awsApplication);
        CreateEventTopic(awsApplication);
    }

    private void CreateEventTopic(AwsAppProject awsApplication)
    {

        var topicName = awsApplication.GetResourceName(eventTopicSuffix);
        var eventTopic = new Topic(this, topicName, new TopicProps()
        {
            TopicName = topicName,
            DisplayName = topicName,
            Fifo = true,
            ContentBasedDeduplication = true
        });
        awsApplication.SetCfOutput(this, $"{eventTopicSuffix}-{arnSuffixValue}", eventTopic.TopicArn);
    }

    public void CreateECRRepoForEventProcessor(AwsAppProject awsApplication)
    {
        var repoName = awsApplication.GetResourceName(EcrRepoSuffix);
        var eventProcessorECRRepo = new Repository(this, repoName, new RepositoryProps()
        {
            RepositoryName = repoName
        });
        awsApplication.SetCfOutput(this, $"{EcrRepoSuffix}-{arnSuffixValue}", eventProcessorECRRepo.RepositoryArn);
        awsApplication.SetCfOutput(this, $"{EcrRepoSuffix}-name", eventProcessorECRRepo.RepositoryName);
    }


    public void SetupTableForNFTRecord(AwsAppProject awsApplication)
    {
        string dynamoDBTableFullName = awsApplication.GetResourceName(dynamoDBTableSuffix);
        var dynamoDbTableProps = new TableProps()
        {
            TableName = dynamoDBTableFullName
        };
        dynamoDbTableProps.PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute()
        {
            Type = Amazon.CDK.AWS.DynamoDB.AttributeType.STRING,
            Name = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel.TokenId)
        };

        dynamoDbTableProps.SortKey = new Amazon.CDK.AWS.DynamoDB.Attribute()
        {
            Type = Amazon.CDK.AWS.DynamoDB.AttributeType.STRING,
            Name = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel.WalletId)
        };
        var table = new Table(this, dynamoDBTableFullName, dynamoDbTableProps);
        awsApplication.SetCfOutput(this, $"{dynamoDBTableSuffix}-{arnSuffixValue}", table.TableArn);
    }
}

