using FluentValidation;
using CodingChallenge.Application.NFT.Base;
namespace CodingChallenge.Application.NFT.Commands.Transfer;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(v => v.TokenId)
            .NotEmpty()
            .Custom((tokenId, context) =>
        {
            if (!tokenId.IsHex())
            {
                context.AddFailure("Token Id must be Hexadecimal");
            }
        });
    }
}