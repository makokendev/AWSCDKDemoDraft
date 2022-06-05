using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Frosting;


namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("DotnetCore-Publish-Task")]
public sealed class DotnetCorePublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        foreach (var projectSetting in context.Config.ProjectSettingsList)
        {
            if (!projectSetting.PublishProject)
            {
                continue;
            }
            var outputDirectory = System.IO.Path.Combine(context.Config.StandardFolders.PublishDir, projectSetting.ProjectName);
            var msBuildSettings = new DotNetMSBuildSettings
            {
                TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
                Verbosity = context.Config.DotnetSettings.DotnetCoreVerbosity
            };
            var settings = new DotNetPublishSettings
            {
                Configuration = context.Config.DotnetSettings.DotnetConfiguration,
                MSBuildSettings = msBuildSettings,
                NoRestore = true,
                OutputDirectory = outputDirectory,
                Verbosity = context.Config.DotnetSettings.DotnetCoreVerbosity
            };

            if (!string.IsNullOrEmpty(context.Config.DotnetSettings.DotnetFramework))
            {
                context.Information($"dotnet publish - framework set as {context.Config.DotnetSettings.DotnetFramework}.");
                settings.Framework = context.Config.DotnetSettings.DotnetFramework;
            }
            else
            {
                context.Information($"dotnet publish  - Using default framework.");
            }
            context.DotNetPublish(context.GetProjectFilePathUsingBuiltinArguments(projectSetting.ProjectName), settings);
        }
        context.DotNetBuildServerShutdown();
    }
}