using System;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using CodingChallenge.Cdk;
using CodingChallenge.Cdk.Extensions;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("Cdk-Infra-Deploy-Task")]
public sealed class CdkInfraStackDeployTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var logPrefix = "CDK Infra Stack";
        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
            if (projectSetting.RunCdkDeploy == true)
            {
                context.Information($"{logPrefix}start cdk deployment for {projectSetting.ProjectName}");
                context.DeployStack(projectSetting, context.Config.AwsApplication.GetDefaultInfraStackName());
            }
            else
            {
                context.Warning($"{logPrefix} deployment is not enabled for this project {projectSetting.ProjectName}");
            }
        }
    }
}
