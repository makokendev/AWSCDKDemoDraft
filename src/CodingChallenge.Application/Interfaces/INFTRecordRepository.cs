using System.Collections.Generic;
using System.Threading.Tasks;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Application.Interfaces;
public interface INFTRecordRepository
{
    Task<List<NFTRecordEntity>> GetByWalletIdAsync(string walletId);
    Task<NFTRecordEntity> GetByTokenIdAsync(string tokenId);
    Task BurnAsync(string id);
    Task MintAsync(NFTRecordEntity nFTEntity);
    Task TransferAsync(NFTRecordEntity nFTEntity, string newWalletId);
    Task ResetAsync();
}
