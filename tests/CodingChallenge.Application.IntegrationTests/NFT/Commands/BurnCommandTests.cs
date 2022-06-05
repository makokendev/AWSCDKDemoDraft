using System.Threading.Tasks;
using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Commands;

public class BurnCommandTests : CQRSTestBase
{
    [Fact]
    public async Task BurnCommandShouldSucceed()
    {
        var mintResponse = await SendMintCommandAsync();
        var postMintResponse = await GetNFTByIdQueryAsync(mintResponse.TokenId);
        var burnResponse = await BurnMintCommandAsync(mintResponse.TokenId);
        var tokenResponseAfterBurn = await GetNFTByIdQueryAsync(mintResponse.TokenId);

        Assert.Equal(mintResponse.WalletId, postMintResponse.WalletId);
        Assert.Equal(mintResponse.TokenId, postMintResponse.TokenId);
        Assert.Equal(mintResponse.TokenId, burnResponse.TokenId);
        Assert.Null(tokenResponseAfterBurn);
    }
}
