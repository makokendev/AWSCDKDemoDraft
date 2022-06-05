using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using CodingChallenge.Application.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;
using CodingChallenge.Application.Exceptions;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;
public class NFTRecordRepository : INFTRecordRepository
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly NFTRecordDataModelDbContext _context;
    public NFTRecordRepository(IMapper mapper, ILogger logger, NFTRecordDataModelDbContext dbContext)
    {
        _mapper = mapper;
        this._logger = logger;
        _context = dbContext;
        _context.Database.EnsureCreated();
    }
    public async Task MintAsync(NFTRecordEntity nFTEntity)
    {
        _logger.LogDebug($"Mint repo action is being executed... Token Id is {nFTEntity.TokenId}");
        var token = await _context.NftDataModel.FindAsync(nFTEntity.TokenId);
        if (token != null)
        {
            throw new NFTTokenAlreadyExistsException($"Token with id {nFTEntity.TokenId} already exists in the database");
        }
        var dataModel = _mapper.Map<NFTRecordEntity, NFTRecordDataModel>(nFTEntity);
        _context.Add(dataModel);
        await _context.SaveChangesAsync();
        _logger.LogDebug($"Mint repo action is successfully executed ... Token Id is {nFTEntity.TokenId}");
    }
    public async Task BurnAsync(string tokenId)
    {
        _logger.LogDebug($"Burn repo action is being executed... Token Id is {tokenId}");
        var result = await _context.NftDataModel.FindAsync(tokenId);
        _context.NftDataModel.Remove(result);
        await _context.SaveChangesAsync();
        _logger.LogDebug($"Burn repo action is successfully executed... Token Id is {tokenId}");
    }

    public async Task<NFTRecordEntity> GetByTokenIdAsync(string tokenId)
    {
        _logger.LogDebug($"Get By Token repo action is being executed... Token Id is {tokenId}");
        var result = await _context.NftDataModel.FindAsync(tokenId);
        var mappedEntity = _mapper.Map<NFTRecordDataModel, NFTRecordEntity>(result);
        _logger.LogDebug($"Get By Token repo action is successfully executed for token with Id {tokenId}. Returning result");
        return mappedEntity;
    }

    public async Task<List<NFTRecordEntity>> GetByWalletIdAsync(string walletId)
    {
        _logger.LogDebug($"Get tokens from wallet repo action is being executed... Wallet Id is {walletId}");
        var result = await _context.NftDataModel.Where(m => m.WalletId == walletId).ToListAsync();
        var mappedEntity = _mapper.Map<List<NFTRecordDataModel>, List<NFTRecordEntity>>(result);
        _logger.LogDebug($"Get tokens from wallet repo action is successfully executed for wallet with Id {walletId}. Returning result");
        return mappedEntity;
    }

    public async Task ResetAsync()
    {
        _logger.LogDebug($"Reset repo action is being executed");
        _context.RemoveRange(_context.NftDataModel);
        await _context.SaveChangesAsync();
        _logger.LogDebug($"Reset repo action is successfully executed");
    }

    public async Task TransferAsync(NFTRecordEntity nFTEntity, string newWalletId)
    {
        _logger.LogDebug($"Transfer repo action is being executed... Token Id is {nFTEntity.TokenId}");
        var dataModel = _mapper.Map<NFTRecordEntity, NFTRecordDataModel>(nFTEntity);
        var token = await _context.NftDataModel.FindAsync(nFTEntity.TokenId);
        if (token == null)
        {
            throw new NFTTokenNotFoundException($"Token with id {nFTEntity.TokenId} does not exist in the database");
        }
        token.WalletId = newWalletId;
        await _context.SaveChangesAsync();
        _logger.LogDebug($"Transfer repo action is successfully executed ... Token Id is {nFTEntity.TokenId}");
    }
}
