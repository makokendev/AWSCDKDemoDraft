using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Application.Exceptions;
using CodingChallenge.Domain.Entities.NFT;
using CodingChallenge.Infrastructure.Repository;
using System;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;
public class NFTRecordDynamoDBRepository : ApplicationDynamoDBBase<NFTRecordDataModel>, INFTRecordRepository
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public NFTRecordDynamoDBRepository(IMapper mapper, ILogger logger, AWSAppProject awsApplication) : base(logger, awsApplication)
    {
        _mapper = mapper;
        this._logger = logger;
    }


    public async Task MintAsync(NFTRecordEntity nFTEntity)
    {
        _logger.LogDebug($"Mint repo action is being executed... Token Id is {nFTEntity.TokenId}");
        var dataModel = _mapper.Map<NFTRecordEntity, NFTRecordDataModel>(nFTEntity);

        await this.SaveAsync(new List<NFTRecordDataModel>(){
            dataModel
        });
        _logger.LogDebug($"Mint repo action is successfully executed ... Token Id is {nFTEntity.TokenId}");
    }
    public async Task BurnAsync(string tokenId)
    {
        _logger.LogDebug($"Burn repo action is being executed... Token Id is {tokenId}");
        var tokenObj = await GetAsync(tokenId);
        if (tokenObj == null)
        {
            throw new NFTTokenNotFoundException($"Token with id {tokenId} does not exist in the database");
        }
        await this.DeleteAsync(new List<NFTRecordDataModel>()
        {
            tokenObj
        });
        _logger.LogDebug($"Burn repo action is successfully executed... Token Id is {tokenId}");
    }

    public async Task<NFTRecordEntity> GetByTokenIdAsync(string tokenId)
    {
        _logger.LogDebug($"Get By Token repo action is being executed... Token Id is {tokenId}");
        var result = await this.GetAsync(tokenId);
        var mappedEntity = _mapper.Map<NFTRecordDataModel, NFTRecordEntity>(result);
        _logger.LogDebug($"Get By Token repo action is successfully executed for token with Id {tokenId}. Returning result");
        return mappedEntity;
    }

    public async Task<List<NFTRecordEntity>> GetByWalletIdAsync(string walletId)
    {
        _logger.LogDebug($"Get tokens from wallet repo action is being executed... Wallet Id is {walletId}");
        var results = await GetBySortKeyAsync(nameof(NFTRecordDataModel.WalletId), walletId);
        var mappedEntity = _mapper.Map<List<NFTRecordDataModel>, List<NFTRecordEntity>>(results);
        _logger.LogDebug($"Get tokens from wallet repo action is successfully executed for wallet with Id {walletId}. Returning result");
        return mappedEntity;
    }

    public async Task ResetAsync()
    {
        await Task.CompletedTask;
        throw new NotSupportedException("Reset Action is not supported at his point");
    }
    public async Task TransferAsync(NFTRecordEntity nFTEntity, string newWalletId)
    {
        _logger.LogDebug($"Transfer repo action is being executed... Token Id is {nFTEntity.TokenId}");
        var dataModel = _mapper.Map<NFTRecordEntity, NFTRecordDataModel>(nFTEntity);
        var token = await GetAsync(nFTEntity.TokenId);
        if (token == null)
        {
            throw new NFTTokenNotFoundException($"Token with id {nFTEntity.TokenId} does not exist in the database");
        }
        token.WalletId = newWalletId;
        await UpdateAsync(new List<NFTRecordDataModel>(){
            token
        });
        _logger.LogDebug($"Transfer repo action is successfully executed ... Token Id is {nFTEntity.TokenId}");
    }
}
