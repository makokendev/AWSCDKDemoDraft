using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodingChallenge.Application.Exceptions;
using CodingChallenge.Application.NFT.Base;
using CodingChallenge.Application.NFT.Queries.Token;
using CodingChallenge.Application.NFT.Queries.Wallet;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CodingChallenge.Console;

public class NFTRecordConsoleRunner
{
    ILogger _logger;
    NFTRecordCommandController _nftRecordCommandHandler;
    public NFTRecordConsoleRunner(ILogger logger, NFTRecordCommandController handler)
    {
        _logger = logger;
        _nftRecordCommandHandler = handler;
    }

    public async Task RunOptionsAsync(CommandLineOptions opts)
    {
        try
        {
            if (!string.IsNullOrEmpty(opts.FilePath))
            {
                await HandleFileReadOptionAsync(opts.FilePath);
            }
            else if (!string.IsNullOrEmpty(opts.InlineJson))
            {
                await HandleInlineJsonOptionAsync(opts.InlineJson);
            }
            else if (!string.IsNullOrEmpty(opts.WalletId))
            {
                await HandleGetWalletDataOptionAsync(opts.WalletId);
            }
            else if (!string.IsNullOrEmpty(opts.TokenId))
                await HandleGetTokenDataOptionAsync(opts.TokenId);
            else if (opts.Reset)
            {
                await HandleResetOptionAsync();
            }
            else
            {
                _logger.LogError("Please input a command");
            }
        }
        catch (RequestValidationException rve)
        {
            _logger.LogError($"Validation error!");
            if (rve.Errors.Any())
            {
                foreach (var error in rve.Errors)
                {
                    _logger.LogError($"error: {error.Key} - {string.Join("-", error.Value)}");
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"CodingChallenge Request: Unhandled Exception. Message: {ex.Message}");
        }
    }



    private async Task HandleResetOptionAsync()
    {
        _logger.LogDebug($"NFTConsoleRunner.Reset command is being processed...");
        var commandResponse = await _nftRecordCommandHandler.ResetAsync(new Application.NFT.Commands.Reset.ResetCommand());
        _logger.LogInformation($"Program was reset");
    }

    private async Task HandleGetTokenDataOptionAsync(string tokenId)
    {
        _logger.LogDebug($"NFTConsoleRunner.HandleGetTokenDataOption is running for {tokenId}");
        var commandResponse = await _nftRecordCommandHandler.GetTokenByIdAsync(new GetNFTsByTokenIdQuery(tokenId));
        if (commandResponse == null)
        {
            _logger.LogInformation($"Token {tokenId} is not owned by any wallet");
        }
        else
        {
            _logger.LogInformation($"Token {tokenId} is owned {commandResponse.WalletId}");
        }
    }

    private async Task HandleGetWalletDataOptionAsync(string walletId)
    {
        _logger.LogDebug($"NFTConsoleRunner.HandleGetWalletDataOption is being processed...");
        var commandResponse = await _nftRecordCommandHandler.GetWalletContentAsync(new GetNFTsFromWalletQuery( walletId));
        if (commandResponse?.Any() == true)
        {
            _logger.LogInformation($"Wallet {walletId} holds {commandResponse.Count} Tokens:");
            foreach (var token in commandResponse)
            {
                _logger.LogInformation($"{token.TokenId}");
            }
        }
        else
        {
            _logger.LogInformation($"Wallet {walletId} holds no Tokens");
        }
    }

    private async Task HandleInlineJsonOptionAsync(string inlineJson)
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

    private async Task HandleFileReadOptionAsync(string filePath)
    {
        _logger.LogDebug($"file is being passed...");
        var listOfCommands = filePath.ParseJsonFile(_logger);
        var listOfCommandResponses = await _nftRecordCommandHandler.ProcessCommandListAsync(listOfCommands);
        _logger.LogInformation($"Read {listOfCommands.Count} transaction(s)");
        foreach (var response in listOfCommandResponses)
        {
            if (!response.IsSuccess)
            {
                _logger.LogError($"Error occurred for {response.TransactionType}. Error message {response.ErrorMessage}");
            }
        }
    }

    public async Task HandleParseErrorAsync(IEnumerable<Error> errs)
    {

        //help requested and version requested are built in and can be ignored.
        if (errs.Any(e => e.Tag != ErrorType.HelpRequestedError && e.Tag != ErrorType.VersionRequestedError))
        {
            foreach (var error in errs)
            {
                _logger.LogWarning($"Command line parameter parse error. {error.ToString()}");
            }
        }
        await Task.CompletedTask;
    }
}