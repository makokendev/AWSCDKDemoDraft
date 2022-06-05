using CodingChallenge.Application.NFT.Base;
using FluentValidation;

namespace CodingChallenge.Application.NFT.Commands.Mint;

public class MintCommandValidator : AbstractValidator<MintCommand>
{
    public MintCommandValidator()
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
        RuleFor(v => v.Address)
            .NotEmpty()
            .Custom((walletId, context) =>
        {
            if (!walletId.IsHex())
            {
                context.AddFailure("Address Id must be Hexadecimal");
            }
        });

    }
}

