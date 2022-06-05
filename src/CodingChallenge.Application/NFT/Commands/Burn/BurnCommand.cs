using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Application.NFT.Base;
using CodingChallenge.Domain.Entities.NFT;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Application.NFT.Commands.Burn;
public record BurnCommand(string TokenId) : NFTTransactionCommandBase(TokenId,NFTTransactionType.Burn), IRequest<BurnCommandResponse>;
public record BurnCommandResponse(string TokenId) : NFTTransactionCommandResponseBase(NFTTransactionType.Burn);

public class BurnCommandHandler : IRequestHandler<BurnCommand, BurnCommandResponse>
{
    public BurnCommandHandler(INFTRecordRepository repo, ILogger logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public INFTRecordRepository _repo { get; }
    public ILogger _logger { get; }

    public IMapper _mapper { get; }

    public async Task<BurnCommandResponse> Handle(BurnCommand request, CancellationToken cancellationToken)
    {
        var entity = new NFTRecordEntity()
        {
            TokenId = request.TokenId,
        };

        await _repo.BurnAsync(request.TokenId);
        _logger.LogDebug($"Dispatching event... response id is {entity.TokenId}...");
        var response = new BurnCommandResponse(request.TokenId);
        _logger.LogDebug($"deleted the entry hopefully response id is {response.TokenId}");
        return response;
    }

}
