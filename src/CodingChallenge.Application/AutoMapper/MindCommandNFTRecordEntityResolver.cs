using AutoMapper;
using CodingChallenge.Application.NFT.Commands.Mint;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Application.AutoMapper;

public class MindCommandNFTRecordEntityResolver : IValueResolver<MintCommand, NFTRecordEntity, NFTWallet>
{
    public NFTWallet Resolve(MintCommand source, NFTRecordEntity destination, NFTWallet member, ResolutionContext context) => new NFTWallet(source.Address);
}
