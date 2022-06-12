using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Docker;
using Cake.Frosting;
using CodingChallenge.Infrastructure.Extensions;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("Docker-Build-Task")]
public sealed class DockerBuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.UseDefaultDockerContext();

        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
             if (projectSetting.DockerImageSettings == null || !projectSetting.DockerImageSettings.Any())
            {
                context.Information($"there is no DockerSettingsList");
                continue;
            }
            foreach (var dockerSetting in projectSetting.DockerImageSettings)
            {
                var dockerFilePath = System.IO.Path.Combine(context.Config.StandardFolders.PublishDirFullPath, projectSetting.ProjectName);
                var tagName = context.Config.AwsApplication.GetResourceName(dockerSetting.DockerRepoNameSuffix);
                var dockerBuildSettings = new DockerImageBuildSettings()
                {
                    Tag = new string[] { $"{tagName}" }
                };
                var dockerFileName = dockerSetting.DockerFileName;

                if (!string.IsNullOrWhiteSpace(dockerFileName))
                {
                    dockerBuildSettings.File = System.IO.Path.Combine(dockerFilePath,dockerFileName);
                }
                context.Information($"docker file path is : {dockerFilePath} --- File name : {dockerFileName}");
                context.DockerBuild(dockerBuildSettings, dockerFilePath);
            }
        }
    }
}
