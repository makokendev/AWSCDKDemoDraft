using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Domain.Base;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;



public class NFTRecordDataModelNFTRecordEntityResolver : IValueResolver<NFTRecordDataModel, NFTRecordEntity, NFTWallet>
{
    public NFTWallet Resolve(NFTRecordDataModel source, NFTRecordEntity destination, NFTWallet member, ResolutionContext context) => new NFTWallet(source.WalletId);
}
public class NFTRecordDataModel : AuditableEntity, IMapFrom<NFTRecordEntity>
{

    [DynamoDBRangeKey]
    public string TokenId { get; set; }
    [DynamoDBHashKey]
    public string WalletId { get; set; }
    [DynamoDBProperty]
    public string Type { get; set; }


    public void Mapping(Profile profile)
    {
        profile.CreateMap<NFTRecordEntity, NFTRecordDataModel>()
            .ForMember(d => d.TokenId, opt => opt.MapFrom(s => s.TokenId))
            .ForMember(d => d.WalletId, opt => opt.MapFrom(s => s.Wallet.WalletId));
        profile.CreateMap<NFTRecordDataModel, NFTRecordEntity>()
            .ForMember(d => d.TokenId, opt => opt.MapFrom(s => s.TokenId))
            .ForMember(d => d.Wallet, opt => opt.MapFrom<NFTRecordDataModelNFTRecordEntityResolver>());
    }
}


