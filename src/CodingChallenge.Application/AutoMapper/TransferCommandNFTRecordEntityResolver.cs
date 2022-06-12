using AutoMapper;
using CodingChallenge.Application.NFT.Commands.Transfer;
using CodingChallenge.Domain.Entities.NFT;

namespace CodingChallenge.Application.AutoMapper;

public class TransferCommandNFTRecordEntityResolver : IValueResolver<TransferCommand, NFTRecordEntity, NFTWallet>
{
    public NFTWallet Resolve(TransferCommand source, NFTRecordEntity destination, NFTWallet member, ResolutionContext context) => new NFTWallet(source.From);
}
