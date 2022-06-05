
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Domain.Entities.NFT;
using MediatR;

namespace CodingChallenge.Application.NFT.Queries.Wallet;
public record GetNFTsFromWalletQuery(string WalletId) : IRequest<List<NFTRecordDto>>;

public class GetNFTsFromWalletQueryHandler : IRequestHandler<GetNFTsFromWalletQuery, List<NFTRecordDto>>
{
    private readonly INFTRecordRepository _repository;
    private readonly IMapper _mapper;

    public GetNFTsFromWalletQueryHandler(INFTRecordRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<NFTRecordDto>> Handle(GetNFTsFromWalletQuery request, CancellationToken cancellationToken)
    {
        var responseEntity = await _repository.GetByWalletIdAsync(request.WalletId);
        return _mapper.Map<List<NFTRecordEntity>, List<NFTRecordDto>>(responseEntity);
    }
}
