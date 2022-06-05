using System;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using CodingChallenge.Cdk;
using CodingChallenge.Cdk.Extensions;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("Cdk-Main-Deploy-Task")]
public sealed class CdkMainStackDeployTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var logPrefix = "CDK Main Stack";
        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
            if (projectSetting.RunCdkDeploy == true)
            {
                context.Information($"{logPrefix}start cdk deployment for {projectSetting.ProjectName}");
                var mainStack = context.Config.AwsApplication.GetDefaultMainStackName();
                context.DeployStack(projectSetting, mainStack);
            }
            else
            {
                context.Warning($"{logPrefix} deployment is not enabled for this project {projectSetting.ProjectName}");
            }
        }
    }
}
