using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using CodingChallenge.Cdk.Stacks;
using Markdig;
using Newtonsoft.Json;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("Upload-Documentation-Storage-Task")]
public sealed class UploadDocumentationToStorage : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        await CreateReadmeWebpage(context);
        await UploadFiles(context);
    }


    private async Task UploadFiles(BuildContext context)
    {
        var s3UploadFiles = new Dictionary<string,string>{
                {"Overview.drawio.html","docs/overview.html"},
                {"README.html","index.html"}
        };
            var docBucketName = InfraStack.GetDocBucketName(context.Config.AwsApplication);

        foreach (var file in s3UploadFiles)
        {
            var sourceFilePath = Path.Combine(context.Config.StandardFolders.RootFullPath,file.Key );
            await UploadFile(context, docBucketName, sourceFilePath, $"{file.Value}");
        }
    }

    private async Task UploadFile(BuildContext context, string bucketName, string sourceFile, string bucketKey)
    {
        context.Information($"uploading {sourceFile} to {bucketName} bucket");
        var s3Client = new AmazonS3Client();
        var result = await s3Client.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = bucketName,
            FilePath = sourceFile,
            Key = bucketKey

        });
        context.Information($"s3 put result is {result.HttpStatusCode}. Json -> {JsonConvert.SerializeObject(result)}");
    }

    private async Task CreateReadmeWebpage(BuildContext context)
    {
        var fileContent = await File.ReadAllTextAsync(Path.Combine(context.Config.StandardFolders.RootFullPath, "README.md"));
        var result = Markdown.ToHtml(fileContent);
        await File.WriteAllTextAsync(Path.Combine(context.Config.StandardFolders.RootFullPath, "README.html"), result);
    }

}
