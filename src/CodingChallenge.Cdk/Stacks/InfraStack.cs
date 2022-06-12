using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SNS;
using CodingChallenge.Cdk.Extensions;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Extensions;

namespace CodingChallenge.Cdk.Stacks;

public sealed class InfraStack : Stack
{
    public const string EcrRepoSuffix = "processor-lambda";
    public const string eventTopicSuffix = "eventtopic";
    // public const string dynamoDBTableSuffix = nameof(Infrastructure.Persistence.NFTRecord.NFTRecordDataModel);
    public const string arnSuffixValue = "arn";

    public InfraStack(Construct parent, string id, IStackProps props, AWSAppProject awsApplication) : base(parent, id, props)
    {
        CreateECRRepoForEventProcessor(awsApplication);
        CreateEventTopic(awsApplication);
        SetupDocumentationS3Bucket(awsApplication);
    }

    //public static string GetDocBucketName(AwsAppProject awsApplication) => awsApplication.GetResourceName("docbucket");
    public static string GetDocBucketName(AWSAppProject awsApplication) => $"awscdkdemo.{awsApplication.DomainName}";
    private void SetupDocumentationS3Bucket(AWSAppProject awsApplication)
    {
        var bucketName = GetDocBucketName(awsApplication);
        var documentationBucket = new Bucket(this, bucketName, new BucketProps
        {
            BucketName = bucketName,
            PublicReadAccess = true,
            WebsiteIndexDocument = "index.html"
            
        });
         awsApplication.SetCfOutput(this, $"docbucket-websiteurl", documentationBucket.BucketWebsiteUrl);
    }

    private void CreateEventTopic(AWSAppProject awsApplication)
    {

        var topicName = awsApplication.GetResourceName(eventTopicSuffix);
        var eventTopic = new Topic(this, topicName, new TopicProps()
        {
            TopicName = topicName,
            DisplayName = topicName,
            Fifo = true,
            ContentBasedDeduplication = true
        });
        awsApplication.SetCfOutput(this, $"{eventTopicSuffix}-{arnSuffixValue}", eventTopic.TopicArn);
    }

    public void CreateECRRepoForEventProcessor(AWSAppProject awsApplication)
    {
        var repoName = awsApplication.GetResourceName(EcrRepoSuffix);
        var eventProcessorECRRepo = new Repository(this, repoName, new RepositoryProps()
        {
            RepositoryName = repoName
        });
        awsApplication.SetCfOutput(this, $"{EcrRepoSuffix}-{arnSuffixValue}", eventProcessorECRRepo.RepositoryArn);
        awsApplication.SetCfOutput(this, $"{EcrRepoSuffix}-name", eventProcessorECRRepo.RepositoryName);
    }
}

