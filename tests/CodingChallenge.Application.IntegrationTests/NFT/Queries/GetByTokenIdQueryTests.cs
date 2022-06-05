using System.Threading.Tasks;
using CodingChallenge.Application.IntegrationTests.NFT.Commands;
using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Queries;

public class GetByTokenIdQueryTests : CQRSTestBase
{
    [Fact]
    public async Task GetNFTByIdQueryShouldSucceed()
    {
        var mintResponse = await SendMintCommandAsync();
        var tokenResponse = await GetNFTByIdQueryAsync(mintResponse.TokenId);

        Assert.Equal(mintResponse.WalletId,tokenResponse.WalletId);
        Assert.Equal(mintResponse.TokenId,tokenResponse.TokenId);
    }
}
