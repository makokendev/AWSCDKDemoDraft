using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Domain.Entities.NFT;
using MediatR;

namespace CodingChallenge.Application.NFT.Queries.Token;

public record GetNFTsByTokenIdQuery(string TokenId) : IRequest<NFTRecordDto>;

public class GetNFTsByTokenIdQueryHandler : IRequestHandler<GetNFTsByTokenIdQuery, NFTRecordDto>
{
    private readonly INFTRecordRepository repo;
    private readonly IMapper _mapper;

    public GetNFTsByTokenIdQueryHandler(INFTRecordRepository context, IMapper mapper)
    {
        repo = context;
        _mapper = mapper;
    }

    public async Task<NFTRecordDto> Handle(GetNFTsByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var responseEntity = await repo.GetByTokenIdAsync(request.TokenId);
        return _mapper.Map<NFTRecordEntity, NFTRecordDto>(responseEntity);
    }
}
