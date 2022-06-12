using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;
using CodingChallenge.Infrastructure.Extensions;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace CodingChallenge.Infrastructure.Repository;

public interface IApplicationDynamoDBBase<T> where T : class
{
    Task DeleteAsync(List<T> dataModelItems);
    Task<T> GetAsync(string id);
    Task SaveAsync(List<T> dataModelItems);
    Task UpdateAsync(List<T> dataModelItems);
}

public class ApplicationDynamoDBBase<T> : DynamoDBRepository, IApplicationDynamoDBBase<T> where T : class
{
    protected virtual DynamoDBOperationConfig DynamoDBOperationConfig { get; }

    public ApplicationDynamoDBBase(ILogger logger, AWSAppProject awsApplication) : base(logger,awsApplication)
    {
        DynamoDBOperationConfig = GetOperationConfig(AwsApplication.GetDynamodbTableName(typeof(T)));
    }

    public async Task DeleteAsync(List<T> dataModelItems)
    {
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        dataModelBatch.AddDeleteItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }

    public async Task<T> GetAsync(string id)
    {
        var result = await Context.LoadAsync<T>(id, DynamoDBOperationConfig);
        return result;
    }
    public async Task<List<T>> GetBySortKeyAsync(string sortKeyField, string sortKeyValue)
    {

        return await Context.ScanAsync<T>(new ScanCondition[] { new ScanCondition(sortKeyField, ScanOperator.Equal, sortKeyValue) }, DynamoDBOperationConfig).GetRemainingAsync();
    }


    public async Task SaveAsync(List<T> dataModelItems)
    {
        Logger.LogInformation($"ApplicationDynamoDBBase... SaveAsync");
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        Logger.LogInformation($"log information... item count {dataModelItems.Count}");
        dataModelBatch.AddPutItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }

    public async Task UpdateAsync(List<T> dataModelItems)
    {
        var dataModelBatch = Context.CreateBatchWrite<T>(DynamoDBOperationConfig);
        dataModelBatch.AddPutItems(dataModelItems);
        await dataModelBatch.ExecuteAsync();
    }

}
