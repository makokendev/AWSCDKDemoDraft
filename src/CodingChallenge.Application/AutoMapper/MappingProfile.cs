using AutoMapper;
using CodingChallenge.Application.NFT.Commands.Mint;
using CodingChallenge.Application.NFT.Commands.Transfer;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Application.AutoMapper;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        typeof(MappingProfile).Assembly.ApplyMappingsFromAssembly(this);
        MapMintCommand();
        MapTransferCommand();
    }

    private void MapMintCommand()
    {
        CreateMap<MintCommand, NFTRecordEntity>()
            .ForMember(dest => dest.TokenId, a => a.MapFrom(o => o.TokenId))
            .ForMember(dest => dest.Wallet, a => a.MapFrom<MindCommandNFTRecordEntityResolver>());
    }
    private void MapTransferCommand()
    {
        CreateMap<TransferCommand, NFTRecordEntity>()
            .ForMember(dest => dest.TokenId, a => a.MapFrom(o => o.TokenId))
            .ForMember(dest => dest.Wallet, a => a.MapFrom<TransferCommandNFTRecordEntityResolver>());
    }


}
