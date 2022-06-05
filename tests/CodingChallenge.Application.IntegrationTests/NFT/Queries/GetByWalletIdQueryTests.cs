using System.Linq;
using System.Threading.Tasks;
using CodingChallenge.Application.IntegrationTests.NFT.Commands;
using Xunit;

namespace CodingChallenge.Application.IntegrationTests.NFT.Queries;

public class GetByWalletIdQueryTests : CQRSTestBase
{
    [Fact]
    public async Task GetNFTByWalletIdQueryShouldSucceed()
    {
        var mint1Response = await SendMintCommandAsync();
        var mint2Response = await SendMintCommandAsync(null, mint1Response.WalletId);
        var getByWalletIdResponse = await GetNFTByWalletIdQueryAsync(mint1Response.WalletId);

        Assert.Equal(getByWalletIdResponse.FirstOrDefault(m=>m.TokenId == mint1Response.TokenId).WalletId,mint1Response.WalletId);
        Assert.Equal(getByWalletIdResponse.FirstOrDefault(m=>m.TokenId == mint2Response.TokenId).WalletId,mint2Response.WalletId);
    
    }
}
