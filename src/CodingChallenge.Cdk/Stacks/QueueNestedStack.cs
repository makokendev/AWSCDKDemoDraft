using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Infrastructure;

namespace CodingChallenge.Cdk.Stacks;
public sealed class QueueNestedStack : Amazon.CDK.NestedStack
{
    public Queue QueueObj { get; private set; }
    public QueueNestedStack(Construct parent, string id, Amazon.CDK.NestedStackProps props, AWSAppProject awsApplication, string namesuffix, bool isFifo, int maxReceiveCount = 10) : base(parent, id, props)
    {
        var deadletterQueue = awsApplication.GetSqsQueue(this, $"{namesuffix}-deadletter", isFifo);
        QueueObj = awsApplication.GetSqsQueue(this, namesuffix, isFifo, new DeadLetterQueue()
        {
            Queue = deadletterQueue,
            MaxReceiveCount = maxReceiveCount
        });
    }
}