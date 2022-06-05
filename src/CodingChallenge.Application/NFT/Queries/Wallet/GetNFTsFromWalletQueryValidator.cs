using FluentValidation;
using CodingChallenge.Application.NFT.Base;
using CodingChallenge.Application.NFT.Queries.Wallet;

namespace CodingChallenge.Application.NFT.Queries.Token;

public class GetNFTsFromWalletQueryValidator : AbstractValidator<GetNFTsFromWalletQuery>
{
    public GetNFTsFromWalletQueryValidator()
    {
        RuleFor(v => v.WalletId)
            .NotEmpty()
            .Custom((walletId, context) =>
        {
            if (!walletId.IsHex())
            {
                context.AddFailure("Wallet Id must be Hexadecimal");
            }
        });
    }
}