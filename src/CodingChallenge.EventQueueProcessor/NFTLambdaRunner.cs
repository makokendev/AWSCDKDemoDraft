using CodingChallenge.Application.NFT.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CodingChallenge.EventQueueProcessor;

public class NFTRecordLambdaRunner
{
    ILogger _logger;
    NFTRecordCommandController _nftRecordCommandHandler;
    public NFTRecordLambdaRunner(ILogger logger, NFTRecordCommandController handler)
    {
        _logger = logger;
        _nftRecordCommandHandler = handler;
    }


    public async Task HandleInlineJsonOptionAsync(string inlineJson)
    {
        _logger.LogDebug($"HandleGetWalletDataOption.HandleInlineJsonOption is being processed... {inlineJson}");
        var token = JToken.Parse(inlineJson);
        List<NFTTransactionCommandBase> listOfCommands = new List<NFTTransactionCommandBase>();
        if (token is JArray)
        {
            listOfCommands.AddRange(inlineJson.ParseListOfTransactionCommands(_logger));
        }
        else if (token is JObject)
        {
            listOfCommands.Add(inlineJson.GetTransactionCommandFromJsonString(_logger));
        }
        if (listOfCommands.Any())
        {
            var listOfCommandResponses = await _nftRecordCommandHandler.ProcessCommandListAsync(listOfCommands);
            _logger.LogInformation($"Read {listOfCommands.Count} transaction(s)");
            foreach (var response in listOfCommandResponses)
            {
                if (!response.IsSuccess)
                {
                    _logger.LogError($"Error occurred for {response.TransactionType}. Error message: {response.ErrorMessage}");
                }
            }
        }
        else
        {
            _logger.LogInformation($"{inlineJson} is not valid json");
        }

    }

}