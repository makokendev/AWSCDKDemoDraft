using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Clean;
using Cake.Common.Tools.DotNetCore.Clean;
using Cake.Frosting;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("DotnetCore-Clean-Task")]
public sealed class DotnetCoreCleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var msCleanSettings = new DotNetCoreCleanSettings
        {
            Verbosity = context.Config.DotnetSettings.DotnetCoreVerbosity
        };

        var settings = new DotNetCleanSettings
        {
            Configuration = context.Config.DotnetSettings.DotnetConfiguration,
        };

        if (!string.IsNullOrEmpty(context.Config.DotnetSettings.DotnetFramework))
        {
            context.Information($"dotnet Clean - framework set as {context.Config.DotnetSettings.DotnetFramework}.");
            settings.Framework = context.Config.DotnetSettings.DotnetFramework;
        }
        else
        {
            context.Information($"dotnet Clean  - Using default framework.");
        }

        foreach (var solution in context.Config.StandardFolders.Solutions)
        {
            context.Information("Cleaning '{0}'...", solution);
            context.DotNetClean(solution.FullPath, settings);
            context.Information("'{0}' has been built.", solution);
        }
        context.DotNetBuildServerShutdown();
    }
}