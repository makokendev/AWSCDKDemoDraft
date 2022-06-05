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

namespace CodingChallenge.Application.NFT.Commands.Transfer;


public record TransferCommand(string TokenId,string From, string To) : NFTTransactionCommandBase(TokenId,NFTTransactionType.Transfer), IRequest<TransferCommandResponse>;
public record TransferCommandResponse(string TokenId, string NewWalletId) : NFTTransactionCommandResponseBase(NFTTransactionType.Transfer);

public class TransferCommandHandler : IRequestHandler<TransferCommand, TransferCommandResponse>
{
    public INFTRecordRepository _repo { get; }
    public ILogger _logger { get; }
    public IMapper _mapper { get; }
    public TransferCommandHandler(INFTRecordRepository repo, ILogger logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<TransferCommandResponse> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var retRec = new TransferCommandResponse(request.TokenId, request.To);
        try
        {
            var entity = _mapper.Map<TransferCommand, NFTRecordEntity>(request);
            entity.LastModified = DateTime.Now;
            entity.LastModifiedBy = "CurrentUserId";
            await _repo.TransferAsync(entity, request.To);
        }
        catch (NFTTokenNotFoundException ex)
        {
            retRec.ErrorMessage = ex.Message;
        }
        return retRec;
    }
}
