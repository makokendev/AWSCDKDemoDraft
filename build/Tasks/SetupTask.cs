using Cake.Common;
using Cake.Frosting;
using CodingChallenge.CakeBuild.Models;
using CodingChallenge.Cdk.Stacks;
using CodingChallenge.Infrastructure;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("Setup-Task")]
public sealed class SetupTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        SetupDeployProjects(context);
    }

    private static void SetupDeployProjects(BuildContext context)
    {
        var awsRegion = context.EnvironmentVariable(Constants.AWS_REGION_ENV_NAME);
        var awsAccountNumber = context.EnvironmentVariable(Constants.AWS_ACCOUNT_NUMBER_ENV_NAME);


        context.Config.ProjectSettingsList.Add(new ProjectSettings()
        {
            ProjectName = "CodingChallenge.Console",
            PublishProject = true
        });
        context.Config.ProjectSettingsList.Add(new ProjectSettings()
        {
            ProjectName = "CodingChallenge.EventQueueProcessor",
            PublishProject = true,
            DockerImageSettings = new System.Collections.Generic.List<ProjectSettings.DockerImageSetting>(){
                new ProjectSettings.DockerImageSetting(){
                    AwsAccountNumber =awsAccountNumber,
                    AwsRegion = awsRegion,
                    DockerFileName = "Dockerfile-Lambda",
                    DockerRepoNameSuffix = InfraStack.EcrRepoSuffix,
                    RunDockerBuild= true,
                    RunDockerPush = true
                }
            }
        });
        context.Config.ProjectSettingsList.Add(new ProjectSettings()
        {
            ProjectName = "CodingChallenge.Cdk",
            PublishProject = true,
            RunCdkDeploy = true
        });
    }
}