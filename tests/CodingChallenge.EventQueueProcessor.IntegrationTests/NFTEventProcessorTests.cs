using System.Threading.Tasks;
using CodingChallenge.Application.NFT.Commands.Mint;
using Newtonsoft.Json;
using Xunit;
using CodingChallenge.Application.IntegrationTests.NFT.Commands;

namespace CodingChallenge.EventQueueProcessor.IntegrationTests;

public class NFTEventProcessorTests : CQRSTestBase
{
    [Fact]
    public async Task HandleAsyncTest()
    {
        var mintTransaction = new MintCommand(GenerateBigIntegerHexadecimal(),GenerateBigIntegerHexadecimal());
        var processor = new EventQueueLambdaClass();
        await processor.HandleAsync(new Amazon.Lambda.SQSEvents.SQSEvent()
        {
            Records = new System.Collections.Generic.List<Amazon.Lambda.SQSEvents.SQSEvent.SQSMessage>(){
                new Amazon.Lambda.SQSEvents.SQSEvent.SQSMessage(){
                    Body = JsonConvert.SerializeObject(mintTransaction)
                }
            }
        },null);
    }
}
