using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Commands;

public class ResetCommandTests : CQRSTestBase
{
    [Fact]
    public async Task ResetCommandShouldSucceed()
    {
        var mintResponse = await SendMintCommandAsync();
        var postMintResponse = await GetNFTByIdQueryAsync(mintResponse.TokenId);

        var targetWalletId = GenerateBigIntegerHexadecimal();
        var transferResponse = await TransferMintCommandAsync(mintResponse.TokenId, mintResponse.WalletId, targetWalletId);
        var getTokenResponseAfterTransfer = await GetNFTByIdQueryAsync(mintResponse.TokenId);
        var resetCommandResponse = await ResetCommandAsync();
        var postResetGetResponse = await GetNFTByIdQueryAsync(mintResponse.TokenId);
        var postResetGetWalletResponse = await GetNFTByWalletIdQueryAsync(targetWalletId);

        Assert.NotEqual(postMintResponse.WalletId, targetWalletId);
        Assert.Equal(mintResponse.WalletId, postMintResponse.WalletId);
        Assert.Equal(mintResponse.TokenId, postMintResponse.TokenId);
        Assert.Equal(transferResponse.NewWalletId, targetWalletId);

        Assert.NotEqual(getTokenResponseAfterTransfer.WalletId, mintResponse.WalletId);
        Assert.Equal(getTokenResponseAfterTransfer.WalletId, targetWalletId);

        Assert.Null(postResetGetResponse);
        Assert.NotNull(postResetGetWalletResponse);
        Assert.False(postResetGetWalletResponse.Any());
    }
}
