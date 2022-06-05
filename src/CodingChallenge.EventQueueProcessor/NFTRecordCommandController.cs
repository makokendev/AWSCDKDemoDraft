using CodingChallenge.Application.NFT.Base;
using CodingChallenge.Application.NFT.Commands.Burn;
using CodingChallenge.Application.NFT.Commands.Mint;
using CodingChallenge.Application.NFT.Queries;
using CodingChallenge.Application.NFT.Commands.Reset;
using CodingChallenge.Application.NFT.Commands.Transfer;
using MediatR;
using Microsoft.Extensions.Logging;
using CodingChallenge.Application.NFT.Queries.Wallet;
using CodingChallenge.Application.NFT.Queries.Token;

namespace CodingChallenge.EventQueueProcessor;
public class NFTRecordCommandController
{
    private readonly ISender _mediator;

    private readonly ILogger _logger;

    public NFTRecordCommandController(ILogger logger, ISender sender)
    {
        _logger = logger;
        _mediator = sender;
    }

    public async Task<List<NFTTransactionCommandResponseBase>> ProcessCommandListAsync(List<NFTTransactionCommandBase> commandList)
    {
        var responseList = new List<NFTTransactionCommandResponseBase>();
        foreach (var command in commandList)
        {
            responseList.Add(await ExecuteTransactionCommandBaseAsync(command));
        }
        return responseList;
    }

    public async Task<NFTTransactionCommandResponseBase> ExecuteTransactionCommandBaseAsync(NFTTransactionCommandBase command)
    {
        if (command is MintCommand)
        {
            _logger.LogDebug($"command with token id {command.TokenId} is MintCommand");
            return await MintAsync(command as MintCommand);
        }
        if (command is BurnCommand)
        {
            _logger.LogDebug($"command with token id {command.TokenId} is BurnCommand");
            return await BurnAsync(command as BurnCommand);
        }
        if (command is TransferCommand)
        {
            _logger.LogDebug($"command with token id {command.TokenId} is Transfer");
            return await TransferAsync(command as TransferCommand);
        }
        return null;
    }

    public async Task<MintCommandResponse> MintAsync(MintCommand mintCommand)
    {
        _logger.LogDebug($"mint command is called for token id {mintCommand.TokenId}");
        return await _mediator.Send<MintCommandResponse>(mintCommand);
    }
    public async Task<BurnCommandResponse> BurnAsync(BurnCommand burnCommand)
    {
        _logger.LogDebug($"burn command is called for token id {burnCommand.TokenId}");
        return await _mediator.Send<BurnCommandResponse>(burnCommand);
    }
    public async Task<ResetCommandResponse> ResetAsync(ResetCommand resetCommand)
    {
        _logger.LogDebug($"Reset command is called for token id {resetCommand}");
        return await _mediator.Send<ResetCommandResponse>(resetCommand);
    }
    public async Task<TransferCommandResponse> TransferAsync(TransferCommand transferCommand)
    {
        _logger.LogDebug($"transfer command is called for token id {transferCommand.TokenId}");
        return await _mediator.Send<TransferCommandResponse>(transferCommand);
    }
    public async Task<List<NFTRecordDto>> GetWalletContentAsync(GetNFTsFromWalletQuery query)
    {
        _logger.LogDebug($"wallet content query is called for token id {query.WalletId}");
        return await _mediator.Send<List<NFTRecordDto>>(query);
    }
    public async Task<NFTRecordDto> GetTokenByIdAsync(GetNFTsByTokenIdQuery query)
    {
        _logger.LogDebug($"token query is called for token id {query.TokenId}");
        return await _mediator.Send<NFTRecordDto>(query);
    }
}
