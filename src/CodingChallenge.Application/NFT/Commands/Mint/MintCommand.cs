using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CodingChallenge.Application.Exceptions;
using CodingChallenge.Application.Interfaces;
using CodingChallenge.Domain.Entities.NFT;
using MediatR;
using Microsoft.Extensions.Logging;
using CodingChallenge.Application.NFT.Base;

namespace CodingChallenge.Application.NFT.Commands.Mint;

public record MintCommand(string TokenId,string Address) : NFTTransactionCommandBase(TokenId,NFTTransactionType.Mint), IRequest<MintCommandResponse>;

public record MintCommandResponse(string TokenId,string WalletId) : NFTTransactionCommandResponseBase(NFTTransactionType.Mint);

public class MintCommandHandler : IRequestHandler<MintCommand, MintCommandResponse>
{
    public MintCommandHandler(INFTRecordRepository repo, ILogger logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public INFTRecordRepository _repo { get; }
    public ILogger _logger { get; }

    public IMapper _mapper { get; }

    public async Task<MintCommandResponse> Handle(MintCommand request, CancellationToken cancellationToken)
    {
        var retRec = new MintCommandResponse(request.TokenId, request.Address);
        try
        {
            var entity = _mapper.Map<MintCommand, NFTRecordEntity>(request);
            entity.Created = DateTime.Now;
            entity.CreatedBy = "CurrentUserId";
            _logger.LogDebug($"handling.... MintCommandHandler... id:{entity.TokenId} -- sortkey:{entity.TokenId}");
            await _repo.MintAsync(entity);

        }
        catch (NFTTokenAlreadyExistsException ex)
        {
            retRec.ErrorMessage = ex.Message;
        }
        return retRec;
    }
}
