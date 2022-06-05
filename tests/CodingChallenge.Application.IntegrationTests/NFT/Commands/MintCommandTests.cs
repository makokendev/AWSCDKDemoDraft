using System.Threading.Tasks;
using CodingChallenge.Application.Exceptions;
using CodingChallenge.Application.NFT.Commands.Mint;
using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Commands;

public class MintCommandTests : CQRSTestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new MintCommand(null,null);
        var exception = await Assert.ThrowsAsync<RequestValidationException>(async () => await Sender.Send(command));
        Assert.True(exception.Errors.Count > 0);
        Assert.NotNull(exception.Errors[nameof(command.TokenId)]);
    }

    [Fact]
    public async Task TokenidShouldBeValidHexadecimal()
    {
        var command = new MintCommand("non-hex",null);
        //assert
        var exception = await Assert.ThrowsAsync<RequestValidationException>(async () => await Sender.Send(command));
        Assert.True(exception.Errors.Count > 0);
        Assert.NotNull(exception.Errors[nameof(command.TokenId)]);
    }
    [Fact]
    public async Task WalletIdIsRequired()
    {
        var tokenId = GenerateBigIntegerHexadecimal();
        var command = new MintCommand(tokenId,null);
        //assert
        RequestValidationException exception = await Assert.ThrowsAsync<RequestValidationException>(async () => await Sender.Send(command));
        Assert.True(exception.Errors.Count > 0);
        Assert.NotNull(exception.Errors[nameof(command.Address)]);
    }

    [Fact]
    public async Task MintTransactionShouldSucceed()
    {
        var tokenId = GenerateBigIntegerHexadecimal();
        var walletId = GenerateBigIntegerHexadecimal();
        var command = new MintCommand(tokenId,walletId);

        var response = await Sender.Send(command);
        Assert.Equal(response.TokenId, tokenId);
        Assert.Equal(response.WalletId, walletId);

    }
}
