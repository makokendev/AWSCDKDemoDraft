using Cake.Common.Diagnostics;
using Cake.Frosting;
using CodingChallenge.Cdk.Extensions;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("Cdk-Database-Deploy-Task")]
public sealed class CdkDatabaseStackDeployTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var logPrefix = "CDK Database Stack";
        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
            if (projectSetting.RunCdkDeploy == true)
            {
                context.Information($"{logPrefix} start cdk deployment for {projectSetting.ProjectName}");
                context.DeployStack(projectSetting, context.Config.AwsApplication.GetDefaultDatabaseStackName());
            }
            else
            {
                context.Warning($"{logPrefix} deployment is not enabled for this project {projectSetting.ProjectName}");
            }
        }
    }
}
