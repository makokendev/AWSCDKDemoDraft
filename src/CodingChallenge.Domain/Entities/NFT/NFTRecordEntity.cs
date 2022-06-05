using CodingChallenge.Domain.Base;

namespace CodingChallenge.Domain.Entities.NFT;
public class NFTRecordEntity : AuditableEntity
{
    public string TokenId { get; set; }
    public NFTWallet Wallet { get; set; }
}
