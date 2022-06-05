using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;
using Cake.Core;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("DotnetCore-Restore-Task")]
public sealed class DotnetCoreRestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetRestoreSettings
        {
            DisableParallel = false,
            NoCache = true,
            Verbosity = context.Config.DotnetSettings.DotnetCoreVerbosity,
            ArgumentCustomization = args => args.Append($"/nodeReuse:false -p:Version={context.Config.AwsApplication.Version} -p:UseSharedCompilation=false")
        };

        foreach (var solution in context.Config.StandardFolders.Solutions)
        {
            context.Information("Restoring NuGet packages for '{0}'...", solution);
            context.DotNetRestore(solution.FullPath, settings);
            context.Information("NuGet packages restored for '{0}'.", solution);
        }
        context.DotNetBuildServerShutdown();
    }

}