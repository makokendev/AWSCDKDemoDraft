using AutoMapper;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Application.NFT.Queries;
public class NFTRecordDto : IMapFrom<NFTRecordEntity>
{
    public string TokenId { get; set; }

    public string WalletId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<NFTRecordEntity, NFTRecordDto>()
            .ForMember(d => d.TokenId, opt => opt.MapFrom(s => s.TokenId))
            .ForMember(d => d.WalletId, opt => opt.MapFrom(s => s.Wallet.WalletId));
   
    }
}
