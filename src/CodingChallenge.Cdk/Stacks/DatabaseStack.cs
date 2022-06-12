using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;
using CodingChallenge.Infrastructure.Persistence.NFTRecord;

namespace CodingChallenge.Cdk.Stacks;

public sealed class DatabaseStack : Stack
{
    public const string arnSuffixValue = "arn";
    public DatabaseStack(Construct parent, string id, IStackProps props, AWSAppProject awsApplication) : base(parent, id, props)
    {
        SetupTableForNFTRecord(awsApplication);
    }

    public void SetupTableForNFTRecord(AWSAppProject awsApplication)
    {

        string dynamoDBTableFullName = awsApplication.GetDynamodbTableName(typeof(NFTRecordDataModel));
        var dynamoDbTableProps = new TableProps()
        {
            TableName = dynamoDBTableFullName
        };
        dynamoDbTableProps.PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute()
        {
            Type = Amazon.CDK.AWS.DynamoDB.AttributeType.STRING,
            Name = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel.WalletId)
        };

        dynamoDbTableProps.SortKey = new Amazon.CDK.AWS.DynamoDB.Attribute()
        {
            Type = Amazon.CDK.AWS.DynamoDB.AttributeType.STRING,
            Name = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel.TokenId)
        };
        var table = new Table(this, dynamoDBTableFullName, dynamoDbTableProps);
        awsApplication.SetCfOutput(this, $"{typeof(NFTRecordDataModel).Name.ToLower()}-{arnSuffixValue}", table.TableArn);
    }
}

