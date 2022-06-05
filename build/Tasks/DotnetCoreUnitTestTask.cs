using System.Linq;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("DotnetCore-UnitTest-Task")]
public sealed class DotnetCoreUnitTestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = context.Config.DotnetSettings.DotnetConfiguration,
            NoRestore = true,
            NoBuild = true
        };
        var projects = context.GetFiles($"{context.Config.StandardFolders.TestsFullPath}/**/*Tests.csproj");
        if (projects.Count == 0)
        {
            projects = context.GetFiles($"{context.Config.StandardFolders.TestsFullPath}/**/*tests.csproj");
        }

        if (projects.Count == 0)
        {
            context.Information("There are no unit tests");
            return;
        }
        foreach (var file in projects)
        {
            var filename = file.FullPath;
            // if (filename.ToLower().Contains("integrationtests"))
            // {
            //     continue;
            // }
            var artifactfilePath = $"{context.Config.StandardFolders.ArtifactsDirFullPath}\\{file.GetFilename()}.xml";
            var loggerCommand = "--logger \"trx;LogFileName=" + artifactfilePath + "\"";

            context.Information("Testing '{0}'...", file);
            var dotNetTestSettings = new DotNetTestSettings()
            {
                ArgumentCustomization = args => args.Append(loggerCommand)
            };
            context.DotNetTest(
                file.GetDirectory().FullPath,
                dotNetTestSettings
            );

            context.Information("'{0}' has been tested.", file);
            context.Information("'{0} artifacts path'.", context.Config.StandardFolders.ArtifactsDirFullPath);

            context.TeamCity().ImportData("vstest", context.Config.StandardFolders.ArtifactsDirFullPath);
        }
        context.DotNetBuildServerShutdown();
    }
}
