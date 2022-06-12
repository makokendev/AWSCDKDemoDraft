using System.Linq;
using Cake.Docker;
using Cake.Frosting;
using CodingChallenge.Infrastructure.Extensions;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("DockerEcrPush-Task")]
public sealed class DockerEcrPushTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.UseDefaultDockerContext();
        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
            if (projectSetting.DockerImageSettings == null || !projectSetting.DockerImageSettings.Any())
            {
                continue;
            }
            foreach (var dockerSetting in projectSetting.DockerImageSettings)
            {
                if (dockerSetting.RunDockerPush == true)
                {
                    var awsApp = context.Config.AwsApplication;
                    context.DockerEcrLogin(dockerSetting.AwsAccountNumber,dockerSetting.AwsRegion);
                    var buildTagName = context.Config.AwsApplication.GetResourceName(dockerSetting.DockerRepoNameSuffix);
                    var ecrTagNameBase = $"{dockerSetting.AwsAccountNumber}.dkr.ecr.{dockerSetting.AwsRegion}.amazonaws.com/{buildTagName}";
                    var ecrTagNameVersion = $"{ecrTagNameBase}:{context.Config.AwsApplication.Version}";
                    var ecrTagNameLatest = $"{ecrTagNameBase}:{context.Config.AwsApplication.Version}";
                    context.DockerTag(buildTagName,ecrTagNameVersion);
                    context.DockerTag(buildTagName,ecrTagNameLatest);
                    context.DockerPush(ecrTagNameVersion);
                    context.DockerPush(ecrTagNameLatest);
                }
            }
        }
    }
}
