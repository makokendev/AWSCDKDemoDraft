using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CodingChallenge.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Application.NFT.Commands.Reset;

public record ResetCommand() : IRequest<ResetCommandResponse>;
public record ResetCommandResponse();


public class ResetCommandHandler : IRequestHandler<ResetCommand, ResetCommandResponse>
{
    public ResetCommandHandler(INFTRecordRepository repo, ILogger logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public INFTRecordRepository _repo { get; }
    public readonly ILogger _logger;

    public readonly IMapper _mapper;

    public async Task<ResetCommandResponse> Handle(ResetCommand request, CancellationToken cancellationToken)
    {
        await _repo.ResetAsync();
        return new ResetCommandResponse();
    }
}
