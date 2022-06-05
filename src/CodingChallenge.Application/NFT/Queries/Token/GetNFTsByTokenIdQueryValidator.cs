using FluentValidation;
using CodingChallenge.Application.NFT.Base;
using CodingChallenge.Application.NFT.Queries;

namespace CodingChallenge.Application.NFT.Queries.Token;

public class GetNFTsByTokenIdQueryValidator : AbstractValidator<GetNFTsByTokenIdQuery>
{
    public GetNFTsByTokenIdQueryValidator()
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