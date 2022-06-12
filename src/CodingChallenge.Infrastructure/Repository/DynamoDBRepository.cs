using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;


namespace CodingChallenge.Infrastructure.Repository;
public abstract class DynamoDBRepository
{
    protected virtual IDynamoDBContext Context { get; }
    protected virtual ILogger Logger { get; }
    protected AWSAppProject AwsApplication {get;}

    public DynamoDBRepository(ILogger logger,AWSAppProject awsApplication)
    {
        Logger = logger;
        AwsApplication = awsApplication;
        Context = new DynamoDBContext(new AmazonDynamoDBClient(this.GetDynamoConfig()));
    }
    protected virtual AmazonDynamoDBConfig GetDynamoConfig()
    {
        var dataStoreUrl = $"http://dynamodb.{AwsApplication.AwsRegion}.amazonaws.com";
        return new AmazonDynamoDBConfig()
        {
            ServiceURL = dataStoreUrl
        };
    }

    protected virtual DynamoDBOperationConfig GetOperationConfig(string tableName)
    {
        Logger.LogInformation($"table name is {tableName}");
        var opConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = tableName
        };
        return opConfig;
    }
}
